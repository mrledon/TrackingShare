import fetch from 'react-native-fetch-polyfill';
import {
    START_FETCH_STORE_TYPE,
    FETCH_ERROR_STORE_TYPE,
    FETCH_SUCCESS_STORE_TYPE,

    START_FETCH_DISTRICS,
    FETCH_ERROR_DISTRICS,
    FETCH_SUCCESS_DISTRICS,

    START_FETCH_PROVINCES,
    FETCH_ERROR_PROVINCES,
    FETCH_SUCCESS_PROVINCES,

    START_FETCH_WARDS,
    FETCH_ERROR_WARDS,
    FETCH_SUCCESS_WARDS
} from './types';

import {
    STORE_TYPE,
    PROVINCES,
    DISTRICS,
    WARDS
} from '../../utils/apis';

//================================================================================
// STORE_TYPE
//================================================================================

function getAllStoreType() {
    return fetch(STORE_TYPE, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        timeout: 5000,
        body: JSON.stringify({
        })
    }).then(res => res.json())
        .then(resJSON => resJSON);
}

const fetchSuccessGetAllStoreType = (dataRes) => {
    return {
        type: FETCH_SUCCESS_STORE_TYPE,
        dataRes
    };
}

const startFetchGetAllStoreType = () => {
    return {
        type: START_FETCH_STORE_TYPE
    };
}

const fetchErrorGetAllStoreType = (error) => {
    return {
        type: FETCH_ERROR_STORE_TYPE,
        error
    };
}

export const fetchDataGetAllStoreType = () => {
    return dispatch => {
        dispatch(startFetchGetAllStoreType());
        return getAllStoreType()
            .then(dataRes => {
                dispatch(fetchSuccessGetAllStoreType(dataRes))
            })
            .catch((error) => {
                console.log(error)
                dispatch(fetchErrorGetAllStoreType(error))
            });
    };
}

//================================================================================
// PROVINCES
//================================================================================

function getAllProvinces() {
    return fetch(PROVINCES, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        timeout: 5000,
        body: JSON.stringify({
        })
    }).then(res => res.json())
        .then(resJSON => resJSON);
}

const fetchSuccessGetAllProvinces = (dataRes) => {
    return {
        type: FETCH_SUCCESS_PROVINCES,
        dataRes
    };
}

const startFetchGetAllProvinces = () => {
    return {
        type: START_FETCH_PROVINCES
    };
}

const fetchErrorGetAllProvinces = (error) => {
    return {
        type: FETCH_ERROR_PROVINCES,
        error
    };
}

export const fetchDataGetAllProvinces = () => {
    return dispatch => {
        dispatch(startFetchGetAllProvinces());
        return getAllProvinces()
            .then(dataRes => {
                dispatch(fetchSuccessGetAllProvinces(dataRes))
            })
            .catch((error) => {
                console.log(error)
                dispatch(fetchErrorGetAllProvinces(error))
            });
    };
}

//================================================================================
// DISTRICS
//================================================================================

function getAllDistrics(_provinceId) {
    return fetch(DISTRICS, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        timeout: 5000,
        body: JSON.stringify({
            ProvinceId: _provinceId
        })
    }).then(res => res.json())
        .then(resJSON => resJSON);
}

const fetchSuccessGetAllDistrics = (dataRes) => {
    return {
        type: FETCH_SUCCESS_DISTRICS,
        dataRes
    };
}

const startFetchGetAllDistrics = () => {
    return {
        type: START_FETCH_DISTRICS
    };
}

const fetchErrorGetAllDistrics = (error) => {
    return {
        type: FETCH_ERROR_DISTRICS,
        error
    };
}

export const fetchDataGetAllDistrics = (_provinceId) => {
    return dispatch => {
        dispatch(startFetchGetAllDistrics());
        return getAllDistrics(_provinceId)
            .then(dataRes => {
                dispatch(fetchSuccessGetAllDistrics(dataRes))
            })
            .catch((error) => {
                console.log(error)
                dispatch(fetchErrorGetAllDistrics(error))
            });
    };
}

//================================================================================
// WARDS
//================================================================================

function getAllWards(_districId) {
    return fetch(WARDS, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        timeout: 5000,
        body: JSON.stringify({
            DistrictId: _districId
        })
    }).then(res => res.json())
        .then(resJSON => resJSON);
}

const fetchSuccessGetAllWards = (dataRes) => {
    return {
        type: FETCH_SUCCESS_WARDS,
        dataRes
    };
}

const startFetchGetAllWards = () => {
    return {
        type: START_FETCH_WARDS
    };
}

const fetchErrorGetAllWards = (error) => {
    return {
        type: FETCH_ERROR_WARDS,
        error
    };
}

export const fetchDataGetAllWards = (_districId) => {
    return dispatch => {
        dispatch(startFetchGetAllWards());
        return getAllWards(_districId)
            .then(dataRes => {
                dispatch(fetchSuccessGetAllWards(dataRes))
            })
            .catch((error) => {
                console.log(error)
                dispatch(fetchErrorGetAllWards(error))
            });
    };
}