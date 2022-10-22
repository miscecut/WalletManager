import React from 'react';

function TransactionCategoriesManagementModal(props) {
    return <div className={`misce-modal-container ${props.show ? 'show' : ''}`}>
        <div className="misce-modal">
            <div className="misce-modal-title-container">
                <p className="misce-modal-title">Edit categories</p>
                <button className="misce-close-button" type="button" onClick={props.closeButtonFunction}></button>
            </div>
            <div className="misce-modal-content">

            </div>
        </div>
    </div>
}

export default TransactionCategoriesManagementModal;