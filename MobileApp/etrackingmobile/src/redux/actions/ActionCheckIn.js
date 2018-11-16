import fetch from 'react-native-fetch-polyfill';
import {
    START_FETCH_CHECKIN,
    FETCH_ERROR_CHECKIN,
    FETCH_SUCCESS_CHECKIN
} from './types';

import {
    CHECKIN
} from '../../utils/apis';

//================================================================================
// CHECKIN
//================================================================================

function checkin(_id, _token, _time, _coor) {
    return fetch(CHECKIN, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        timeout: 5000,
        body: JSON.stringify({
            Id: _id,
            Token: _token,
            AttendanceStart: _time,
            StartCoordinates: _coor
        })
    }).then(res => res.json())
        .then(resJSON => resJSON);
}

const fetchSuccess = (dataRes) => {
    return {
        type: FETCH_SUCCESS_CHECKIN,
        dataRes
    };
}

const startFetch = () => {
    return {
        type: START_FETCH_CHECKIN
    };
}

const fetchError = (error) => {
    return {
        type: FETCH_ERROR_CHECKIN,
        error
    };
}

export const fetchDataCheckIn = (_id, _token, _time, _coor) => {
    return dispatch => {
        dispatch(startFetch());
        return checkin(_id, _token, _time, _coor)
            .then(dataRes => {
                dispatch(fetchSuccess(dataRes))
            })
            .catch((error) => {
                console.log(error)
                dispatch(fetchError(error))
            });
    };
}