import React from 'react';
//css
import './Account.css';
//utils
import { formatMoneyAmount } from './../../../jsutils/beautifiers.js';

function Account(props) {

    //FUNCTIONS

    function getLogoClassName(accountTypeName) {
        if (accountTypeName === 'Bank account')
            return 'bank-account';
        else if (accountTypeName === 'Cash')
            return 'cash';
        else //TODO: GENERIC LOGO
            return 'cash';
    }

    //RENDERING

    //render component
    return <div className="misce-account-card">
        <div className={`misce-account-card-icon ${getLogoClassName(props.account.accountType.name)} ${props.account.isActive ? '' : 'inactive'}`}></div>
        <div className="misce-account-card-info-container">
            <p className={`misce-account-name ${props.account.isActive ? '' : 'inactive'}`}>{props.account.name}</p>
            <p className={`misce-account-amount ${props.account.actualAmount >= 0 ? 'positive' : 'negative'}`}>{formatMoneyAmount(props.account.actualAmount)} &euro;</p>
        </div>
    </div>
}

export default Account;