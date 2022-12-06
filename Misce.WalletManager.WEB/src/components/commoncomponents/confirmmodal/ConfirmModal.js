import React from 'react';
//css
import './ConfirmModal.css';

//INPUT PARAMETERS:
//show: if true, the modal is shown
//title: the title in the top of the modal
//message: the confirm message
//confirmParameter: the object to be passed into the confirmButtonFunction
//INPUT FUNCTIONS:
//closeButtonFunction: the function that is called when the X or the CANCEL button is pressed
//confirmButtonFunction: the function that is called when CONFIRM is pressed
function ConfirmModal(props) {

    //RENDERING

    //component render
    return <div className={`misce-modal-container ${props.show ? 'show' : ''}`}>
        <div className="misce-modal">
            <div className="misce-modal-title-container">
                <p className="misce-modal-title">{props.title == '' || props.title == null ? 'Conferma' : props.title}</p>
                <button className="misce-close-button" type="button" onClick={props.closeButtonFunction}></button>
            </div>
            <div className="misce-confirm-modal-content">
                <p className="misce-modal-confirm-message">{props.message}</p>
                <button className="misce-btn" type="button" onClick={() => props.confirmButtonFunction(props.confirmParameter)}>CONFIRM</button>
                <button className="misce-btn" type="button" onClick={props.closeButtonFunction}>CANCEL</button>
            </div>
        </div>
    </div>
}

export default ConfirmModal;