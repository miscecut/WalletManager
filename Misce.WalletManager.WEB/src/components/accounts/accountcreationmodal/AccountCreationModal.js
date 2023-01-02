import React, { useState, useEffect } from 'react';
//css
import './AccountCreationModal.css';
//api
import {
    getApiBaseUrl,
    getGetCommonSettings
} from '../../../jsutils/apirequests.js';
//error handling
import { getErrorMap } from "../../../jsutils/errorhandling.js";

function AccountCreationModal(props) {

    //STATE

    //the selectable account types
    const [accountTypes, setAccountTypes] = useState([]);
    //the account creation form to be passed to the api
    const [account, setAccount] = useState({
        accountTypeId: '',
        name: '',
        description: '',
        status: 'ACTIVE',
        initialAmount: 0.0
    });

    //EFFECTS

    //load the account types
    useEffect(() => {
        fetch(getApiBaseUrl() + 'accounttypes', getGetCommonSettings(props.token))
            .then(res => {
                if (res.ok)
                    res.json().then(data => {
                        setAccountTypes(data);
                    });
            });
    }, []);

    //FUNCTIONS

    const submitHandler = e => {
        e.preventDefault();
        props.accountCreationSubmitFunction(account);
    }

    //RENDERING

    const errorMap = getErrorMap(props.errors);

    //render component
    return <div className={`misce-modal-container ${props.show ? 'show' : ''}`}>
        <div className="misce-modal">
            <div className="misce-modal-title-container">
                <p className="misce-modal-title">Account creation</p>
                <button className="misce-close-button" type="button" onClick={props.closeButtonFunction}></button>
            </div>
            <div className="misce-modal-content">
                <form className="misce-account-form-container" onSubmit={submitHandler}>
                    <div className="misce-input-container">
                        <label className="misce-input-label">Account type:</label>
                        <select className={`misce-select ${errorMap['accounttypeid'] != null ? 'misce-input-error' : ''}`} value={account.accountTypeId} onChange={e => setAccount({ ...account, accountTypeId: e.target.value })}>
                            <option value=""></option>
                            {accountTypes.map(at => <option key={at.id} className="misce-option" value={at.id}>{at.name}</option>)}
                        </select>
                        <p className="misce-input-error-message">{errorMap['accounttypeid']}</p>
                    </div>
                    <div className="misce-input-container">
                        <label className="misce-input-label" htmlFor="name">Name:</label>
                        <input className={`misce-input ${errorMap['name'] != null ? 'misce-input-error' : ''}`} type="text" name="name" id="name" onChange={e => setAccount({ ...account, name: e.target.value })} value={account.name} required></input>
                        <p className="misce-input-error-message">{errorMap['name']}</p>
                    </div>
                    <div className="misce-input-container">
                        <label className="misce-input-label" htmlFor="initialAmount">Initial amount:</label>
                        <input min="0" step=".01" className={`misce-input ${errorMap['initialamount'] != null ? 'misce-input-error' : ''}`} type="number" name="initialAmount" id="initialAmount" onChange={e => setAccount({ ...account, initialAmount: e.target.value })} value={account.initialAmount} required></input>
                        <p className="misce-input-error-message">{errorMap['initialamount']}</p>
                    </div>
                    <div className="misce-input-container">
                        <label className="misce-input-label">Status:</label>
                        <select className={`misce-select ${errorMap['isactive'] != null ? 'misce-input-error' : ''}`} value={account.status} onChange={e => setAccount({ ...account, status: e.target.value })}>
                            <option value="ACTIVE">Active</option>
                            <option value="INACTIVE">Inactive</option>
                        </select>
                        <p className="misce-input-error-message">{errorMap['isactive']}</p>
                    </div>
                    <div className="misce-input-container account-description">
                        <label className="misce-input-label" htmlFor="description">Description:</label>
                        <input className={`misce-input ${errorMap['description'] != null ? 'misce-input-error' : ''}`} type="text" name="description" id="description" onChange={e => setAccount({ ...account, description: e.target.value })} value={account.description}></input>
                        <p className="misce-input-error-message">{errorMap['description']}</p>
                    </div>
                    <div className="misce-input-container label-margin-fix misce-account-create-modal-button-container">
                        <button className="misce-btn" type="submit">add</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
}

export default AccountCreationModal;