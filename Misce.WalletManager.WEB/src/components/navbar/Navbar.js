import React from 'react';
import './Navbar.css';
import AppLogo from '../../images/app-logo-text.png'

export default function Navbar(props) {
    return <nav className="navbar navbar-expand-lg navbar-dark bg-dark sticky-top">
        <div className="container-fluid">
            <a className="navbar-brand" href="#">
                <img src={AppLogo} alt="Wallet Manager Logo" height="35" className="d-inline-block align-text-top"></img>
            </a>
            <div className="collapse navbar-collapse" id="navbarNav">
                {
                    props.isUserLoggedIn
                    ?
                    <ul className="navbar-nav fs-5">
                        <li className="nav-item">
                            <a className={`nav-link ${props.activePage === 'DASHBOARD' ? 'active' : ''}`} href="#">Dashboard</a>
                        </li>
                        <li className="nav-item">
                            <a className={`nav-link ${props.activePage === 'TRANSACTIONS' ? 'active' : ''}`} href="#">Transactions</a>
                        </li>
                        <li className="nav-item">
                            <a className={`nav-link ${props.activePage === 'ACCOUNTS' ? 'active' : ''}`} href="#">Accounts</a>
                        </li>
                    </ul>
                    :
                    ''
                }
                {
                    props.isUserLoggedIn
                        ?
                        <ul className="navbar-nav fs-5 ms-auto">
                            <li className="nav-item">
                                <a className="nav-link" href="#"><i class="fa-solid fa-user"></i> {props.username}</a>
                            </li>
                            <li className="nav-item">
                                <a className="nav-link" href="#"><i class="fa-solid fa-arrow-right-from-bracket"></i> Logout</a>
                            </li>
                        </ul>
                        :
                        <ul className="navbar-nav fs-5 ms-auto">
                            <li className="nav-item">
                                <a className={`nav-link ${props.activePage === 'LOGIN' ? 'active' : ''}`} href="#">Login <i class="fa-solid fa-arrow-right-to-bracket"></i></a>
                            </li>
                            <li className="nav-item">
                                <a className={`nav-link ${props.activePage === 'REGISTER' ? 'active' : ''}`} href="#">Register</a>
                            </li>
                        </ul>
                }
            </div>
        </div>
    </nav>
}