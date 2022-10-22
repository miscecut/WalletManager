import React, { useState } from 'react';
//css
import './MutuallyExclusivePills.css';

//accepted params:
//elements: an array of object with at least: { id: ..., name:... }
//selectedElementId: an id of an element in the list
//selectElement: the click event on a pill
function MutuallyExclusivePills(props) {
    //render component
    return <div className="misce-mup-container">
        {props.elements.map(element => <button
            key={element.id}
            className={`misce-btn ${props.selectedElementId == element.id ? 'active' : ''}`}
            onClick={() => props.selectElement(element.id)}
            type="button">{element.name}</button>)}
    </div>
}

export default MutuallyExclusivePills;