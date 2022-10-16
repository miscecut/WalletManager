import React, { useState } from 'react';
import { getErrorMap } from "../../jsutils/errorhandling.js"

//the component accepts a register function and, eventually, a list of the errors to show
function RegisterForm({ register, registrationErrors }) {
    const [newUser, setNewUser] = useState({ username: '', password: '', confirmPassword: '' });

    const submitHandler = e => {
        e.preventDefault();
        //call the register api
        register(newUser);
    }

    const errorMap = getErrorMap(registrationErrors);

    //component rendering
    return <form onSubmit={submitHandler} className="misce-central-form-container">
        <h1 className="misce-login-title">Sign up</h1>
        <div className="w-100">
            <label className="misce-input-label" htmlFor="username">Username:</label>
            <input className={`misce-input ${errorMap['username'] != null ? 'misce-input-error' : ''}`} type="text" name="username" id="username" onChange={e => setNewUser({ ...newUser, username: e.target.value })} value={newUser.username} required></input>
            <p className="misce-input-error-message">{errorMap['username']}</p>
        </div>
        <div className="w-100">
            <label className="misce-input-label" htmlFor="password">Password:</label>
            <input className={`misce-input ${errorMap['password'] != null ? 'misce-input-error' : ''}`} type="password" name="password" id="password" onChange={e => setNewUser({ ...newUser, password: e.target.value })} value={newUser.password} required></input>
            <p className="misce-input-error-message">{errorMap['password']}</p>
        </div>
        <div className="w-100">
            <label className="misce-input-label" htmlFor="confirm-password">Confirm password:</label>
            <input className={`misce-input ${errorMap['confirmpassword'] != null ? 'misce-input-error' : ''}`} type="password" name="confirm-password" id="confirm-password" onChange={e => setNewUser({ ...newUser, confirmPassword: e.target.value })} value={newUser.confirmPassword} required></input>
            <p className="misce-input-error-message">{errorMap['confirmpassword']}</p>
        </div>
        <div className="w-100 mt-more">
            <button className="misce-btn" type="submit">REGISTER</button>
        </div>
    </form>
}

export default RegisterForm;