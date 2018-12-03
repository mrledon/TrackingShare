import fetch from 'react-native-fetch-polyfill';
import {
    START_FETCH_CHECKOUT,
    FETCH_ERROR_CHECKOUT,
    FETCH_SUCCESS_CHECKOUT
} from './types';

import {
    CHECKOUT
} from '../../utils/apis';

//================================================================================
// CHECKOUT
//================================================================================

function checkout(_id, _token, _time, _coor) {
    return fetch(CHECKOUT, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        timeout: 5000,
        body: JSON.stringify({
            Id: _id,
            Token: _token,
            AttendanceEnd: _time,
            EndCoordinates: _coor
        })
    }).then(res => res.json())
        .then(resJSON => resJSON);
}

const fetchSuccess = (dataRes) => {
    return {
        type: FETCH_SUCCESS_CHECKOUT,
        dataRes
    };
}

const startFetch = () => {
    return {
        type: START_FETCH_CHECKOUT
    };
}

const fetchError = (error) => {
    return {
        type: FETCH_ERROR_CHECKOUT,
        error
    };
}

export const fetchDataCheckOut = (_id, _token, _time, _coor) => {
    return dispatch => {
        dispatch(startFetch());
        return checkout(_id, _token, _time, _coor)
            .then(dataRes => {
                dispatch(fetchSuccess(dataRes))
            })
            .catch((error) => {
                console.log(error)
                dispatch(fetchError(error))
            });
    };
}