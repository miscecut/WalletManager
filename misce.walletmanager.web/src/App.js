import React, { useState } from 'react';
import './App.css';
import Navbar from './components/navbar/Navbar.js';
import LoginForm from './components/loginform/LoginForm.js';
import Dashboard from './components/dashboard/Dashboard.js';

function App() {
    const [user, setUser] = useState({ username: '', token: '' });

    const Login = details => {
        fetch('https://localhost:7264/api/login', {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    username: details.username,
                    password: details.password
                })
            })
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