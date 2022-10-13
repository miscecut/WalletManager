import React from 'react';
import './Navbar.css';
import AppLogo from '../../images/app-logo-text.png'

export default function Navbar () {
    return <nav className="navbar navbar-expand-lg navbar-dark bg-dark sticky-top">
        <div className="container-fluid">
            <a className="navbar-brand" href="#">
                <img src={AppLogo} alt="Wallet Manager Logo" height="35" className="d-inline-block align-text-top"></img>
            </a>
            <div className="collapse navbar-collapse" id="navbarNav">
                <ul className="navbar-nav fs-5">
                    <li className="nav-item">
                        <a className="nav-link active" href="#">Dashboard</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="#">Transactions</a>
                    </li>
                    <li className="nav-item">
                        <a className="nav-link" href="#">Accounts</a>
                    </li>
                </ul>
            </div>
        </div>
    </nav>
}