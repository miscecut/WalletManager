import React from 'react';
import './Navbar.css';
import AppLogo from '../../images/app-logo-text.png'

export default function Navbar(props) {
    return <nav className="navbar navbar-expand-lg navbar-dark bg-dark sticky-top">
        <div className="container-fluid">
            <a className="navbar-brand" href="#">
                <img src={AppLogo} alt="Wallet Manager Logo" height="35" className="d-inline-block align-text-top"></img>
            </a>
            <div className="collapse navbar-collapse fs-5" id="navbarNav">
                {props.isUserLoggedIn ?
                    <ul className="navbar-nav">
                        <li className="nav-item">
                            <a className={`nav-link ${props.activePage === 'DASHBOARD' ? 'active' : ''}`} onClick={() => props.changePage('DASHBOARD')} href="#">Dashboard</a>
                        </li>
                        <li className="nav-item">
                            <a className={`nav-link ${props.activePage === 'TRANSACTIONS' ? 'active' : ''}`} onClick={() => props.changePage('TRANSACTIONS')} href="#">Transactions</a>
                        </li>
                        <li className="nav-item">
                            <a className={`nav-link ${props.activePage === 'ACCOUNTS' ? 'active' : ''}`} onClick={() => props.changePage('ACCOUNTS')} href="#">Accounts</a>
                        </li>
                    </ul>
                    :
                    ''
                }
                {props.isUserLoggedIn ?
                    <ul className="navbar-nav ms-auto">
                        <li className="nav-item">
                            <a className="nav-link" href="#"><i className="fa-solid fa-user"></i> {props.username}</a>
                        </li>
                        <li className="nav-item">
                            <a className="nav-link" href="#" onClick={props.logout}><i className="fa-solid fa-arrow-right-from-bracket"></i> Logout</a>
                        </li>
                    </ul>
                    :
                    <ul className="navbar-nav ms-auto">
                        <li className="nav-item">
                            <a className={`nav-link ${props.activePage === 'LOGIN' ? 'active' : ''}`} onClick={() => props.changePage('LOGIN')} href="#">Login <i className="fa-solid fa-arrow-right-to-bracket"></i></a>
                        </li>
                        <li className="nav-item">
                            <a className={`nav-link ${props.activePage === 'REGISTER' ? 'active' : ''}`} onClick={() => props.changePage('REGISTER')} href="#">Register</a>
                        </li>
                    </ul>
                }
            </div>
        </div>
    </nav>
}