import React from 'react';
//css
import './Transactions.css';
//components
import Transaction from './../transaction/Transaction.js';

function Transactions(props) {
    return <div className="misce-transactions-page-container">
        <div className="misce-card misce-transactions-container">
            
        </div>
        <div className="misce-card misce-transactions-filters">
            <button class="misce-btn w-100" type="button">aggiungi transazione</button>
        </div>
    </div>
}

export default Transactions;