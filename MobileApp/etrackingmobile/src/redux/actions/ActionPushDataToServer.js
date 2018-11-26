import fetch from 'react-native-fetch-polyfill';
import {
    START_FETCH_PUSH_DATA_TO_SERVER,
    FETCH_ERROR_PUSH_DATA_TO_SERVER,
    FETCH_SUCCESS_PUSH_DATA_TO_SERVER,
    IS_CHECK
} from './types';

import {
    PUSH_DATA_TO_SERVER
} from '../../utils/apis';

//================================================================================
// PUSH_DATA_TO_SERVER
//================================================================================

function pushDataToServer(_id, _code, _code2, _date, _masterStoreId, _token, _trackSessionId, _posmNumber, _img) {

    const data = new FormData();
    data.append('Id', _id);
    data.append('Code', _code);
    data.append('Code2', _code2);
    data.append('Date', _date);
    data.append('Token', _token);
    data.append('TrackSessionId', _trackSessionId);
    data.append('PosmNumber', _posmNumber);
    data.append('Photo', _img);

    return fetch(PUSH_DATA_TO_SERVER, {
        method: 'POST',
        headers: {
            'Content-Type': 'multipart/form-data',
        },
        timeout: 5000,
        body: data
    }).then(res => res.json())
        .then(resJSON => resJSON);
}

const fetchSuccess = (dataRes) => {
    return {
        type: FETCH_SUCCESS_PUSH_DATA_TO_SERVER,
        dataRes
    };
}

const startFetch = () => {
    return {
        type: START_FETCH_PUSH_DATA_TO_SERVER
    };
}

const fetchError = (error) => {
    return {
        type: FETCH_ERROR_PUSH_DATA_TO_SERVER,
        error
    };
}

export const check = (isCheck)=>{
    return {
        type: IS_CHECK,
        isCheck
    };
}

export const fetchPushDataToServer = (_id, _code, _code2, _date, _masterStoreId, _token, _trackSessionId, _posmNumber, _img) => {
    return dispatch => {
        dispatch(startFetch());
        return pushDataToServer(_id, _code, _code2, _date, _masterStoreId, _token, _trackSessionId, _posmNumber, _img)
            .then(dataRes => {
                dispatch(fetchSuccess(dataRes))
            })
            .catch((error) => {
                console.log(error)
                dispatch(fetchError(error))
            });
    };
}