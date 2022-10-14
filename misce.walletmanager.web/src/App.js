import React, { useState } from 'react';
//components
import Navbar from './components/navbar/Navbar.js';
import RegisterForm from './components/registerform/RegisterForm.js';
import LoginForm from './components/loginform/LoginForm.js';
import Dashboard from './components/dashboard/Dashboard.js';
//api
import { getApiBaseUrl, getLoginPostSettings, getRegisterPostSettings } from './jsutils/apirequests.js';

function App() {
    //app's state, with only username & token for the api
    const [user, setUser] = useState({ username: '', token: '' });
    //app's selected page
    const [activePage, setActivePage] = useState({ activePageName: 'LOGIN' }); //the app start at the login page

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
                else if (res.status == 401)
                    console.log('nope');
        });
    }

    //logout function
    const logout = () => {
        setUser({
            username: '',
            token: ''
        })
    }

    //change page function, this is called by the nav-links in the navbar, each one with its own pageName
    const changePage = pageName => {
        let activePageOld = { ...activePage };
        activePageOld.activePageName = pageName;
        setActivePage(activePageOld);
    }

    //this function returns the component based on the page name provided
    const getPage = pageName => {
        if (pageName == 'LOGIN')
            return <LoginForm login={login} />
        if (pageName == 'REGISTER')
            return <RegisterForm />
        return <Dashboard />
    }

    //app component rendering
    return (
        <div className="container-fluid px-0">
            <Navbar
                activePage={activePage.activePageName} //the name of the page link to be enlighted
                isUserLoggedIn={user.username != ''} //if false, only the login & register are shown
                username={user.username} //the name to be shown in the navbar
                logout={logout} //logout function, binded to the logout button
                changePage={changePage} //change page function, binded with every nav-link in the navbar
            />
            {getPage(activePage.activePageName)}
        </div>
    );
}

export default App;