import React, { useState } from 'react';
//css
import './AccountsPage.css';
//api
import {
    getApiBaseUrl,
    getGetCommonSettings
} from './../../../jsutils/apirequests.js';

function AccountsPage({ token }) {

    //STATE

    //the user's accounts
    const [accounts, setAccounts] = useState([]);

    //EFFECTS

    //get the user's accounts, this function is called only at the page startup
    useEffect(() => {
        //retrieve accounts
        fetch(getApiBaseUrl() + 'accounts', getGetCommonSettings(token))
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
            
        </div>
}

export default AccountsPage;