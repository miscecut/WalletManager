import React from 'react';
import './Navbar.css';
import AppLogo from '../../images/app-logo.png'

export default function Navbar(props) {
    return <header>
        <img className="nav-logo" src={AppLogo} alt="Wallet Manager Logo"></img>
        <nav>
            {props.isUserLoggedIn ?
                <ul className="nav-option-list">
                    <li className={`nav-option ${props.activePage === 'DASHBOARD' ? 'active' : ''}`}>
                        <a onClick={() => props.changePage('DASHBOARD')} href="#">Dashboard</a>
                    </li>
                    <li className={`nav-option ${props.activePage === 'TRANSACTIONS' ? 'active' : ''}`}>
                        <a onClick={() => props.changePage('TRANSACTIONS')} href="#">Transactions</a>
                    </li>
                    <li className={`nav-option ${props.activePage === 'ACCOUNTS' ? 'active' : ''}`}>
                        <a onClick={() => props.changePage('ACCOUNTS')} href="#">Accounts</a>
                    </li>
                </ul>
                :
                ''
            }
            {props.isUserLoggedIn ?
                <ul className="nav-option-list">
                    <li className="nav-option">
                        <a href="#"><i className="fa-solid fa-user"></i> {props.username}</a>
                    </li>
                    <li className="nav-option">
                        <a onClick={props.logout} href="#"><i className="fa-solid fa-arrow-right-from-bracket"></i> Logout</a>
                    </li>
                </ul>
                :
                <ul className="nav-option-list">
                    <li className={`nav-option ${props.activePage === 'LOGIN' ? 'active' : ''}`}>
                        <a onClick={() => props.changePage('LOGIN')} href="#"><i className="fa-solid fa-arrow-right-to-bracket"></i> Login</a>
                    </li>
                    <li className={`nav-option ${props.activePage === 'REGISTER' ? 'active' : ''}`}>
                        <a onClick={() => props.changePage('REGISTER')} href="#">Register</a>
                    </li>
                </ul>
            }
        </nav>
    </header>
}