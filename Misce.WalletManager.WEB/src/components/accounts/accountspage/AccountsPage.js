import React, { useState, useEffect } from 'react';
//css
import './AccountsPage.css';
//components
import Account from './../account/Account.js';
import AccountCreationModal from './../accountcreationmodal/AccountCreationModal.js';
import NoElementsCard from './../../commoncomponents/noelementscard/NoElementsCard.js';
//api
import {
    getApiBaseUrl,
    getGetCommonSettings
} from '../../../jsutils/apirequests.js';

function AccountsPage(props) {

    //STATE

    //the user's accounts
    const [accounts, setAccounts] = useState([]);
    //account creation errors
    const [accountCreationErrors, setAccountCreationErrors] = useState({ errors: [] });
    //the modals state
    const [modals, setModals] = useState({
        accountCreateModalIsOpen: false
    });

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

    //FUNCTIONS

    //open the account creation modal
    const openAccountCreationModal = () => setModals({ ...modals, accountCreateModalIsOpen: true });
    //closes the account creation modal
    const closeAccountCreationModal = () => setModals({ ...modals, accountCreateModalIsOpen: false });

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
        <div className="misce-account-create-button-container ">
            <button className="misce-btn" type="button" onClick={openAccountCreationModal}>add account</button>
        </div>
        <AccountCreationModal
            show={modals.accountCreateModalIsOpen}
            errors={accountCreationErrors.errors}
            closeButtonFunction={closeAccountCreationModal}
        />
    </div>
}

export default AccountsPage;