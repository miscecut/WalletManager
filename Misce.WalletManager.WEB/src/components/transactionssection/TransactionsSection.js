import React from 'react';
//css
import './TransactionsSection.css';
//components
import Transaction from './../transaction/Transaction.js'
//utils
import { formatMoneyAmount } from './../../jsutils/beautifiers.js';

function TransactionsSection({ title, transactions }) {

    //FUNCTIONS

    const getTotalProfit = () => {
        let tot = 0;
        transactions.forEach(t => {
            if (t.fromAccount === null && t.toAccount !== null)
                tot += t.amount;
        });
        return tot;
    }

    const getTotalExpense = () => {
        let tot = 0;
        transactions.forEach(t => {
            if (t.fromAccount !== null && t.toAccount === null)
                tot += t.amount;
        });
        return tot;
    }

    //RENDERING

    //component render
    return <div className="misce-transactions-section">
        <div className="misce-transactions-section-header">
            <hr className="misce-transactions-section-header-line"></hr>
            <div className="misce-transactions-section-header-title">
                <p>{title}</p>
                <p className="tot-profit">{formatMoneyAmount(getTotalProfit())} &euro;</p>
                <p className="tot-expense">{formatMoneyAmount(getTotalExpense())} &euro;</p>
            </div>
            <hr className="misce-transactions-section-header-line"></hr>
        </div>
        {transactions.map(t => <Transaction key={t.id} transaction={{ ...t, dateTime: new Date(t.dateTime).toLocaleString().replace(',', ' -') }} />)}
    </div>
}

export default TransactionsSection;