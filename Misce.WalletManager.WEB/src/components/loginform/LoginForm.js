import React, { useState } from 'react';
import { getErrorMap } from "../../jsutils/errorhandling.js"

function LoginForm({ login, loginErrors }) {

    //STATE

    //the login form
    const [details, setDetails] = useState({ username: '', password: '' });

    //FUNCTIONS

    const submitHandler = e => {
        e.preventDefault();
        login(details);
    }

    //RENDERING

    const errorMap = getErrorMap(loginErrors);

    //render component
    return <form onSubmit={submitHandler} className="misce-central-form-container">
            <h1 className="misce-login-title">Welcome</h1>
            <div className="misce-input-container">
                <label className="misce-input-label" htmlFor="username">Username:</label>
                <input className="misce-input" type="text"  name="username" id="username" onChange={e => setDetails({ ...details, username: e.target.value })} value={details.username} required></input>
            </div>
            <div className="misce-input-container">
                <label className="misce-input-label" htmlFor="password">Password:</label>
                <input className={`misce-input ${errorMap['password'] != null ? 'misce-input-error' : ''}`} type="password" name="password" id="password" onChange={e => setDetails({ ...details, password: e.target.value })} value={details.password} required></input>
                <p className="misce-input-error-message">{errorMap['password']}</p>
            </div>
            <div className="misce-input-container mt-more">
                <button className="misce-btn" type="submit">LOGIN</button>
            </div>
        </form>
}

export default LoginForm;