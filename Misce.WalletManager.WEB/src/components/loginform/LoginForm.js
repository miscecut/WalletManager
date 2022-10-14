import React, { useState } from 'react';

function LoginForm({ Login }) {
    const [details, setDetails] = useState({ username: '', password: '' });

    const submitHandler = e => {
        e.preventDefault();
        Login(details);
    }

    return <div className="container mt-5">
        <form onSubmit={submitHandler}>
            <div className="row justify-content-center mt-3">
                <div className="col-xl-4 col-lg-6 col-md-8">
                    <label htmlFor="username" className="form-label text-white">Username</label>
                    <input type="text" className="form-control" name="username" id="username" onChange={e => setDetails({ ...details, username: e.target.value })} value={details.username} required></input>
                </div>
            </div>
            <div className="row justify-content-center mt-3">
                <div className="col-xl-4 col-lg-6 col-md-8">
                    <label htmlFor="password" className="form-label text-white">Password</label>
                    <input type="password" className="form-control" name="password" id="password" onChange={e => setDetails({ ...details, password: e.target.value })} value={details.password} required></input>
                </div>
            </div>
            <div className="row justify-content-center mt-5">
                <div className="col-xl-4 col-lg-6 col-md-8">
                    <button type="submit" className="btn btn-outline-light w-100">LOGIN</button>
                </div>
            </div>
        </form>
    </div>
}

export default LoginForm;