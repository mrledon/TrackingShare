import fetch from 'react-native-fetch-polyfill';
import {
    START_FETCH_LOGIN,
    FETCH_ERROR_LOGIN,
    FETCH_SUCCESS_LOGIN
} from './types';

import {
    LOGIN
} from '../../utils/apis';

//================================================================================
// LOGIN
//================================================================================

function login(_email, _password) {
    return fetch(LOGIN, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        timeout: 5000,
        body: JSON.stringify({
            Email: _email,
            Password: _password
        })
    }).then(res => res.json())
        .then(resJSON => resJSON);
}

const fetchSuccess = (dataRes) => {
    return {
        type: FETCH_SUCCESS_LOGIN,
        dataRes
    };
}

const startFetch = () => {
    return {
        type: START_FETCH_LOGIN
    };
}

const fetchError = (error) => {
    return {
        type: FETCH_ERROR_LOGIN,
        error
    };
}

export const fetchDataLogin = (_email, _password) => {
    return dispatch => {
        dispatch(startFetch());
        return login(_email, _password)
            .then(dataRes => {
                dispatch(fetchSuccess(dataRes))
            })
            .catch((error) => {
                console.log(error)
                dispatch(fetchError(error))
            });
    };
}