import React from 'react';
//css
import './TransactionsSection.css';
//components
import Transaction from './../transaction/Transaction.js'

function TransactionsSection({ title, transactions }) {
    return <div className="misce-transactions-section">
        <div className="misce-transactions-section-header">TITOLOooo------------------</div>
        {transactions.map(t => <Transaction transaction={{ ...t, dateTime: new Date(t.dateTime).toLocaleString().replace(',', ' -') }} />)}
    </div>
}

export default TransactionsSection;