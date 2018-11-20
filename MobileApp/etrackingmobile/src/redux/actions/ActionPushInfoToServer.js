import fetch from 'react-native-fetch-polyfill';
import {
    START_FETCH_PUSH_INFO_TO_SERVER,
    FETCH_ERROR_PUSH_INFO_TO_SERVER,
    FETCH_SUCCESS_PUSH_INFO_TO_SERVER
} from './types';

import {
    PUSH_INFO_TO_SERVER
} from '../../utils/apis';

//================================================================================
// PUSH_INFO_TO_SERVER
//================================================================================

function pushInfoToServer(item) {

    return fetch(PUSH_INFO_TO_SERVER, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        timeout: 5000,
        body: JSON.stringify({
            Id: item.Id,
            MaterStoreName: item.MaterStoreName,
            PhoneNumber: item.PhoneNumber,
            HouseNumber: item.HouseNumber,
            StreetNames: item.StreetNames,
            ProvinceId: item.ProvinceId,
            DistrictId: item.DistrictId,
            WardId: item.WardId,
            Lat: item.Lat,
            Lng: item.Lng,
            Note: item.Note,
            Region: item.Region,
            StoreType: item.StoreType,
            MasterStoreId: item.MasterStoreId,
            Date: item.Date,
            Token: item.Token,
            StoreStatus: item.StoreStatus
        })
    }).then(res => res.json())
        .then(resJSON => resJSON);
}

const fetchSuccess = (dataRes) => {
    return {
        type: FETCH_SUCCESS_PUSH_INFO_TO_SERVER,
        dataRes
    };
}

const startFetch = () => {
    return {
        type: START_FETCH_PUSH_INFO_TO_SERVER
    };
}

const fetchError = (error) => {
    return {
        type: FETCH_ERROR_PUSH_INFO_TO_SERVER,
        error
    };
}

export const fetchPushInfoToServer = (item) => {
    return dispatch => {
        dispatch(startFetch());
        return pushInfoToServer(item)
            .then(dataRes => {
                dispatch(fetchSuccess(dataRes))
            })
            .catch((error) => {
                console.log(error)
                dispatch(fetchError(error))
            });
    };
}