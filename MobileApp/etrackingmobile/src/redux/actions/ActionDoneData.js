import fetch from 'react-native-fetch-polyfill';
import {
    START_FETCH_DONE_DATA,
    FETCH_ERROR_DONE_DATA,
    FETCH_SUCCESS_DONE_DATA
} from './types';

import {
    DONE_DATA
} from '../../utils/apis';

//================================================================================
// DONE_DATA
//================================================================================

function getDoneData(_id) {
    return fetch(DONE_DATA, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        timeout: 5000,
        body: JSON.stringify({
            Id: _id
        })
    }).then(res => res.json())
        .then(resJSON => resJSON);
}

const fetchSuccess = (dataRes) => {
    return {
        type: FETCH_SUCCESS_DONE_DATA,
        dataRes
    };
}

const startFetch = () => {
    return {
        type: START_FETCH_DONE_DATA
    };
}

const fetchError = (error) => {
    return {
        type: FETCH_ERROR_DONE_DATA,
        error
    };
}

export const fetchGetDoneData = (_id) => {
    return dispatch => {
        dispatch(startFetch());
        return getDoneData(_id)
            .then(dataRes => {
                dispatch(fetchSuccess(dataRes))
            })
            .catch((error) => {
                console.log(error)
                dispatch(fetchError(error))
            });
    };
}