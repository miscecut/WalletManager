import React from 'react';
//css
import './NoElementsCard.css';
//images
import NothingImage from './../../../images/nothing-gray-2.png';

function NoElementsCard({ message = 'No elements found', vertical = true }) {
    //render component
    return <div className="misce-no-elements-card">
        <img className="misce-no-elements-card-image" src={NothingImage}></img>
        <p className="misce-no-elements-card-message">{message}</p>
    </div>
}

export default NoElementsCard;