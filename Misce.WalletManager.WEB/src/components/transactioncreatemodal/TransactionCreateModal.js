import React, { useState, useEffect } from 'react';
//api
import {
    getApiBaseUrl,
    getGetCommonSettings
} from '../../jsutils/apirequests.js';

//the transaction createion/update form
//inputs in the props:
//-show: true if the modal is shown
//-transactionId: not null and populated if a transaction was selected to be updated, null if a new transaction must be created
function TransactionCreateModal(props) {

    //FUNCTIONS

    //returns an empty transaction form
    const getEmptyTransactionForm = () => {
        return {
            id: '', //uneusefull if a transaction must be created
            title: '',
            accountFromId: '',
            accountToId: '',
            transactionSubCategoryId: '',
            description: '',
            dateTime: new Date()
        };
    }

    //STATE

    //the transaction type to be updated/created
    const [transactionType, setTransactionType] = useState({ transactionTypeId: 0 });
    //the transaction form to be passed to the api
    const [transaction, setTransaction] = useState(getEmptyTransactionForm())

    //EFFECTS

    useEffect(() => {
        //update
        if (props.transactionId != null) {
            //retrieve the transaction data
            fetch(getApiBaseUrl() + 'transactions/' + props.transactionId, getGetCommonSettings(props.token))
                .then(res => {
                    if (res.ok)
                        res.json().then(data => {
                            console.log(data);
                        });
                });
        }
        //create
        else {
            setTransaction(getEmptyTransactionForm());
            if (transactionType.transactionTypeId !== 0)
                setTransactionType({ ...transactionType, transactionTypeId: 0 });
        }
    }, [props.transactionId]);

    //RENDERING

    //render component
    return <div className={`misce-modal-container ${props.show ? 'show' : ''}`}>
        <div className="misce-modal">
            <div className="misce-modal-title-container">
                <p className="misce-modal-title">Transaction creation</p>
                <button className="misce-close-button" type="button" onClick={props.closeButtonFunction}></button>
            </div>
            <div className="misce-modal-content">
                <div className="misce-transaction-types-container">
                </div>
            </div>
        </div>
    </div>
}

export default TransactionCreateModal;