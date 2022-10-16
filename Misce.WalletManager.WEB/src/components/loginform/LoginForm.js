import React, { useState } from 'react';

function LoginForm({ login }) {
    const [details, setDetails] = useState({ username: '', password: '' });

    const submitHandler = e => {
        e.preventDefault();
        login(details);
    }

    return <form onSubmit={submitHandler} className="misce-central-form-container">
            <h1 className="misce-login-title">Welcome</h1>
            <div className="w-100">
                <label className="misce-input-label" htmlFor="username">Username:</label>
                <input className="misce-input" type="text"  name="username" id="username" onChange={e => setDetails({ ...details, username: e.target.value })} value={details.username} required></input>
            </div>
            <div className="w-100">
                <label className="misce-input-label" htmlFor="password">Password:</label>
                <input className="misce-input" type="password" name="password" id="password" onChange={e => setDetails({ ...details, password: e.target.value })} value={details.password} required></input>
            </div>
            <div className="w-100 mt-more">
                <button className="misce-btn" type="submit">LOGIN</button>
            </div>
        </form>
}

export default LoginForm;