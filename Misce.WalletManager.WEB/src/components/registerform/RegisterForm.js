import React, { useState } from 'react';

function RegisterForm({ register }) {
    const [newUser, setNewUser] = useState({ username: '', password: '', confirmPassword: '' });

    const submitHandler = e => {
        e.preventDefault();
        register(newUser);
    }

    return <form onSubmit={submitHandler} className="misce-central-form-container">
        <h1 className="misce-login-title">Registration Form</h1>
        <div className="w-100">
            <label className="misce-input-label" htmlFor="username">Username:</label>
            <input className="misce-input" type="text" name="username" id="username" onChange={e => setNewUser({ ...newUser, username: e.target.value })} value={newUser.username} required></input>
        </div>
        <div className="w-100">
            <label className="misce-input-label" htmlFor="password">Password:</label>
            <input className="misce-input" type="password" name="password" id="password" onChange={e => setNewUser({ ...newUser, password: e.target.value })} value={newUser.password} required></input>
        </div>
        <div className="w-100">
            <label className="misce-input-label" htmlFor="confirm-password">Confirm password:</label>
            <input className="misce-input" type="password" name="confirm-password" id="confirm-password" onChange={e => setNewUser({ ...newUser, confirmPassword: e.target.value })} value={newUser.confirmPassword} required></input>
        </div>
        <div className="w-100 mt-more">
            <button className="misce-btn" type="submit">REGISTER</button>
        </div>
    </form>
}

export default RegisterForm;