import React, { useState, useEffect } from 'react';
//error handling
import { getErrorMap } from "../../../jsutils/errorhandling.js";

function AccountCreationModal(props) {

    //RENDERING

    const errorMap = getErrorMap(props.errors);

    //render component
    return <div className={`misce-modal-container ${props.show ? 'show' : ''}`}>
        <div className="misce-modal">
            <div className="misce-modal-title-container">
                <p className="misce-modal-title">Account creation</p>
                <button className="misce-close-button" type="button" onClick={props.closeButtonFunction}></button>
            </div>
        </div>
        <div className="misce-modal-content">

        </div>
    </div>
}

export default AccountCreationModal;