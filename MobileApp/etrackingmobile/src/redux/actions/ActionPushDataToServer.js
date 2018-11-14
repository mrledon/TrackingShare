import fetch from 'react-native-fetch-polyfill';
import {
    START_FETCH_PUSH_DATA_TO_SERVER,
    FETCH_ERROR_PUSH_DATA_TO_SERVER,
    FETCH_SUCCESS_PUSH_DATA_TO_SERVER
} from './types';

import {
    PUSH_DATA_TO_SERVER
} from '../../utils/apis';

//================================================================================
// PUSH_DATA_TO_SERVER
//================================================================================

function pushDataToServer(_id, _code, _date, _masterStoreId, _token, _trackSessionId, _posmNumber, _img) {

    const data = new FormData();
    data.append('Id', _id);
    data.append('Code', _code);
    data.append('Date', _date);
    data.append('MasterStoreId', _masterStoreId);
    data.append('Token', _token);
    data.append('TrackSessionId', '5dc9c77b-7282-42cc-9446-1960fd430d8b');
    data.append('PosmNumber', '2');
    data.append('Photo', _img);

    // var img= {
    //     uri: _img,
    //     type: 'image/jpeg',
    //     name: 'testPhotoName'
    //   };

    //   var img2= {
    //     uri: _img,
    //     type: 'image/jpeg',
    //     name: 'testPhotoName2'
    //   };

    // data.append('Id', '305478924');
    // data.append('Code', 'DEFAULT');
    // data.append('Date', '12/11/2018');
    // data.append('MasterStoreId', '65863be6-896b-48dd-8b8a-9e065b149461');
    // data.append('Token', 'ebea44c6-1704-4eb6-a4c7-432ddad846e6');
    // data.append('Photo', img);
    // data.append('Photo2', img2);

    return fetch(PUSH_DATA_TO_SERVER, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
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

export const fetchPushDataToServer = (_id, _code, _date, _masterStoreId, _token, _trackSessionId, _posmNumber,_img) => {
    return dispatch => {
        dispatch(startFetch());
        return pushDataToServer(_id, _code, _date, _masterStoreId, _token, _trackSessionId, _posmNumber, _img)
            .then(dataRes => {
                dispatch(fetchSuccess(dataRes))
            })
            .catch((error) => {
                console.log(error)
                dispatch(fetchError(error))
            });
    };
}