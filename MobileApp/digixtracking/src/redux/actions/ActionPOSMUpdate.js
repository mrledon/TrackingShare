import fetch from 'react-native-fetch-polyfill';
import {
    START_FETCH_POSM_UPDATE,
    FETCH_ERROR_POSM_UPDATE,
    FETCH_SUCCESS_POSM_UPDATE
} from './types';

import {
    POSM_UPDATE
} from '../../utils/apis';

//================================================================================
// POSM_UPDATE
//================================================================================

function posmUpdate(_id) {
    return fetch(POSM_UPDATE+ '?trackSessionId=' + _id, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        timeout: 5000,
        // body: {
        //     trackSessionId: _id,
        // }
    }).then(res => res.json())
        .then(resJSON => resJSON);
}

const fetchSuccess = (dataRes) => {
    return {
        type: FETCH_SUCCESS_POSM_UPDATE,
        dataRes
    };
}

const startFetch = () => {
    return {
        type: START_FETCH_POSM_UPDATE
    };
}

const fetchError = (error) => {
    return {
        type: FETCH_ERROR_POSM_UPDATE,
        error
    };
}

export const fetchDataPosmUpdate = (_id) => {
    return dispatch => {
        dispatch(startFetch());
        return posmUpdate(_id)
            .then(dataRes => {
                dispatch(fetchSuccess(dataRes))
            })
            .catch((error) => {
                console.log(error)
                dispatch(fetchError(error))
            });
    };
}