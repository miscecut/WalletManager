import React, { useState } from 'react';
//components
import Navbar from './components/navbar/Navbar.js';
import RegisterForm from './components/registerform/RegisterForm.js';
import LoginForm from './components/loginform/LoginForm.js';
import Dashboard from './components/dashboard/Dashboard.js';
import TransactionsPage from './components/transactionspage/TransactionsPage.js';
//api
import { getApiBaseUrl, getLoginPostSettings, getRegisterPostSettings } from './jsutils/apirequests.js';

function App() {
    //app's state, with only username & token for the api
    const [user, setUser] = useState({ username: '', token: '' });
    //app's selected page
    const [activePage, setActivePage] = useState({ activePageName: 'LOGIN', errors: [] }); //the app starts at the login page

    const register = newUser => {
        fetch(getApiBaseUrl() + 'register', getRegisterPostSettings(newUser))
            .then(res => {
                //if the registration was succesfull
                if (res.ok) {
                    //try to login directly
                    login({
                        username: newUser.username,
                        password: newUser.password
                    });
                }
                //registration failed due to some wrong input or the username being already in use
                else if (res.status === 422 || res.status === 409) {
                    res.json().then(data => changePage('REGISTER', data.errors));
                }
            });
    }

    //login function
    const login = details => {
        fetch(getApiBaseUrl() + 'login', getLoginPostSettings(details))
            .then(res => {
                //if the login was succesfull
                if (res.ok) {
                    res.json().then(data => {
                        //save logged user's data
                        setUser({
                            username: data.username,
                            token: data.token
                        });
                        //show dashboard
                        changePage('DASHBOARD');
                    });
                }
                //unauthorized, login failed
                else if (res.status === 401) {
                    res.json().then(data => changePage('LOGIN', data.errors));
                }
        });
    }

    //logout function
    const logout = () => {
        setUser({
            username: '',
            token: ''
        });
        //show login
        changePage('LOGIN');
    }

    //change page function, this is called by the nav-links in the navbar, each one with its own pageName
    const changePage = (pageName, errors = []) => {
        let activePageOld = { ...activePage };
        activePageOld.activePageName = pageName;
        activePageOld.errors = errors;
        setActivePage(activePageOld);
    }

    //this function returns the component based on the page name provided
    const getPage = (pageName) => {
        if (pageName === 'LOGIN')
            return <LoginForm login={login} loginErrors={activePage.errors} />
        if (pageName === 'REGISTER')
            return <RegisterForm register={register} registrationErrors={activePage.errors} />
        if (pageName === 'TRANSACTIONS')
            return <TransactionsPage token={user.token} />
        return <Dashboard />
    }

    //app component rendering
    return (
        <div>
            <Navbar
                activePage={activePage.activePageName} //the name of the page link to be enlighted
                isUserLoggedIn={user.username !== ''} //if false, only the login & register are shown
                username={user.username} //the name to be shown in the navbar
                logout={logout} //logout function, binded to the logout button
                changePage={changePage} //change page function, binded with every nav-link in the navbar
            />
            <div className="misce-container">
                {getPage(activePage.activePageName)}
            </div>
        </div>
    );
}

export default App;