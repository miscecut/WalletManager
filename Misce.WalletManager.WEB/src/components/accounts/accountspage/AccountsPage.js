import React, { useState, useEffect } from 'react';
//css
import './AccountsPage.css';
//components
import Account from './../account/Account.js';
import NoElementsCard from './../../commoncomponents/noelementscard/NoElementsCard.js';
//api
import {
    getApiBaseUrl,
    getGetCommonSettings
} from './../../../jsutils/apirequests.js';

function AccountsPage(props) {

    //STATE

    //the user's accounts
    const [accounts, setAccounts] = useState([]);

    //EFFECTS

    //get the user's accounts, this function is called only at the page startup
    useEffect(() => {
        //retrieve accounts
        fetch(getApiBaseUrl() + 'accounts/history', getGetCommonSettings(props.token))
            .then(res => {
                if (res.ok)
                    res.json().then(data => {
                        setAccounts(data);
                    });
            });
    }, []);

    //RENDERING

    //component rendering
    return <div className="misce-accounts-page-container">
        {
            accounts.length
            ? 
            accounts.map(a => <Account key={a.id} account={a}></Account>)
            :
            <NoElementsCard message="No accounts found" />
        }
        <div>
            <button className="misce-btn" type="button" onClick={() => props.openAccountCreationModal()}>add account</button>
        </div>
    </div>
}

export default AccountsPage;