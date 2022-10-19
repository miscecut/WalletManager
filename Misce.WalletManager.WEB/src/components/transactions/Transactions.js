import React, { useState } from 'react';
//css
import './Transactions.css';
//components
//import Transaction from './../transaction/Transaction.js';

function Transactions(props) {
    //groupBy: 'DAYS', 'WEEKS', 'MONTHS'
    //transactionType: null, '', 'PROFIT', 'EXPENSE', 'TRANSFER'
    const [filters, setFilters] = useState({ groupBy: 'DAYS', transactionType: null, });

    //function getAccountFilterSelect() {
    //}

    function getTransactionCategoriesSelect() {
        let token = props.token;
        
    }

    //component render
    return <div className="misce-transactions-page-container">
        <div className="misce-card misce-transactions-container">
            
        </div>
        <div className="misce-card misce-transactions-filters">
            <button className="misce-btn w-100" type="button">aggiungi transazione</button>
            <button className="misce-btn w-100" type="button">gestisci categorie</button>
            <div className="misce-input-container">
                <label className="misce-input-label" htmlFor="username">Group transactions:</label>
                <select className="misce-select" value={filters.groupBy} onChange={e => setFilters({ ...filters, groupBy: e.target.value })}>
                    <option value="DAYS">By day</option>
                    <option value="WEEKS">By Week</option>
                    <option value="MONTHS">By Month</option>
                </select>
            </div>
            <div className="misce-input-container">
                <label className="misce-input-label" htmlFor="username">Transaction type:</label>
                <select className="misce-select" value={filters.transactionType} onChange={e => setFilters({ ...filters, transactionType: e.target.value })}>
                    <option value="">All</option>
                    <option value="PROFIT">Profit</option>
                    <option value="EXPENSE">Expense</option>
                    <option value="TRANSFER">Transfer</option>
                </select>
            </div>
        </div>
    </div>
}

export default Transactions;