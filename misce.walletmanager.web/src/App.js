import React, { useState } from 'react';
//components
import Navbar from './components/navbar/Navbar.js';
import LoginForm from './components/loginform/LoginForm.js';
import Dashboard from './components/dashboard/Dashboard.js';
//api
import { getApiBaseUrl, getLoginPostSettings } from './jsutils/apirequests.js';

function App() {
    const [user, setUser] = useState({ username: '', token: '' });

    const Login = details => {
        fetch(getApiBaseUrl() + 'login', getLoginPostSettings(details))
        .then(res => {
            if (res.ok) {
                setUser({
                    username: res.username,
                    token: res.token
                });
            }
            else if (res.status == 401)
                console.log('nope');
        });
    }

    const Logout = () => {
        setUser({
            username: '',
            token: ''
        })
    }

    const GetPage = pageName => {
        if (pageName == 'DASHBOARD')
            return <Dashboard />
    }

    return (
        <div className="container-fluid px-0">
            <Navbar
                activePage='DASHBOARD'
                isUserLoggedIn={user.username != ''}
                username={user.username}
                Logout={Logout}
            />
            {user.username != '' ?
                GetPage('DASHBOARD')
                :
                <LoginForm
                    Login={Login}
                />
            }
        </div>
    );
}

export default App;