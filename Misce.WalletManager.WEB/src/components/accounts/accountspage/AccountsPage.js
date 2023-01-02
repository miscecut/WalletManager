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
    getGetCommonSettings,
    getAccountCreatePostSettings
} from '../../../jsutils/apirequests.js';

function AccountsPage(props) {

    //STATE

    //the user's accounts
    const [accounts, setAccounts] = useState([]);
    //account creation errors
    const [accountCreationErrors, setAccountCreationErrors] = useState([]);
    //the modals state
    const [modals, setModals] = useState({
        accountCreateModalIsOpen: false
    });
    //this state is used to force the update of the accounts (in order to force the update, theese counters must be increased)
    const [forceUpdate, setForceUpdate] = useState({ accounts: 0 });

    //EFFECTS

    //refresh the user's accounts (or load them at page startup)
    useEffect(() => {
        //retrieve accounts
        fetch(getApiBaseUrl() + 'accounts/history', getGetCommonSettings(props.token))
            .then(res => {
                if (res.ok)
                    res.json().then(data => {
                        setAccounts(data);
                    });
            });
    }, [forceUpdate.accounts]);

    //FUNCTIONS

    //open the account creation modal
    const openAccountCreationModal = () => setModals({ ...modals, accountCreateModalIsOpen: true });
    //closes the account creation modal
    const closeAccountCreationModal = () => setModals({ ...modals, accountCreateModalIsOpen: false });

    //create a new account
    const createAccount = account => {
        //the transaction category gets created
        fetch(getApiBaseUrl() + 'accounts', getAccountCreatePostSettings(account, props.token))
            .then(res => {
                //if the operation was succesfull...
                if (res.status === 201)
                    res.json().then(() => {
                        //...close the modal
                        closeAccountCreationModal();
                        //and show the transactions again by force their refresh
                        setForceUpdate({ ...forceUpdate, accounts: forceUpdate.accounts + 1 });
                        //clean errors, if there are
                        if (accountCreationErrors.length != 0)
                            setAccountCreationErrors([]);
                    });
                else if (res.status === 422)
                    res.json().then(data => {
                        setAccountCreationErrors(data.errors);
                    });
            });
    }

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
        <div className="misce-account-create-button-container">
            <button className="misce-btn" type="button" onClick={openAccountCreationModal}>add account</button>
        </div>
        <AccountCreationModal
            token={props.token}
            show={modals.accountCreateModalIsOpen}
            errors={accountCreationErrors}
            closeButtonFunction={closeAccountCreationModal}
            accountCreationSubmitFunction={createAccount}
        />
    </div>
}

export default AccountsPage;