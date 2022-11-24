import React from 'react';
//chart-js-2
import { Line } from 'react-chartjs-2';
import {
    Chart as ChartJS,
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Title,
    Tooltip,
    Legend,
} from 'chart.js';
//css
import './Account.css';
//utils
import { formatMoneyAmount } from './../../../jsutils/beautifiers.js';

//chart-js-2 setup
ChartJS.register(
    CategoryScale,
    LinearScale,
    PointElement,
    LineElement,
    Title,
    Tooltip,
    Legend
);

//component
function Account(props) {

    //LINE GRAPH DATA & OPTIONS
    const chartData = {
        labels: props.account.accountAmountHistory.map(val => val.atDate),
        datasets: [
            {
                label: 'Amount',
                backgroundColor: '#ea80fc',
                borderColor: '#ea80fc',
                data: props.account.accountAmountHistory.map(val => val.amount)
            }
        ]
    };
    const chartOptions = {
        maintainAspectRatio: false
    };

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
        <div>
            <Line
                data={chartData}
                options={chartOptions}
                height={'170px'}
            />
        </div>
    </div>
}

export default Account;