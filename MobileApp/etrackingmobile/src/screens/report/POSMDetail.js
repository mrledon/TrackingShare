import React, { Component } from 'react';
import {
    View, StyleSheet, Image, TouchableOpacity,
    Dimensions, ScrollView, Alert, AsyncStorage,
    CameraRoll, BackHandler
} from 'react-native';
import { Text, Input, Item, Form, Textarea } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import RNPickerSelect from 'react-native-picker-select';
import Spinner from 'react-native-loading-spinner-overlay';
import { RNCamera } from 'react-native-camera';
import RadioGroup from 'react-native-radio-buttons-group';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import {
    fetchDataGetAllStoreType,
    fetchDataGetAllDistrics,
    fetchDataGetAllProvinces,
    fetchDataGetAllWards,
    fetchDataGetStoreByCode
} from '../../redux/actions/ActionPOSMDetail';

import { fetchPushInfoToServer } from '../../redux/actions/ActionPushInfoToServer'

const { width, height } = Dimensions.get("window");
var moment = require('moment');
var RNFS = require('react-native-fs');

const CARD_WIDTH = width / 2 - 40;
const CARD_HEIGHT = CARD_WIDTH - 20;

const CARD_WIDTH_2 = width / 3 - 20;
const CARD_HEIGHT_2 = CARD_WIDTH_2 - 10;

class POSMDetail extends Component {
    constructor(props) {
        super(props);
        this.inputRefs = {};
        this.state = {
            isSave: false,
            isCamera: false,
            isPreview: false,
            isMain: true,
            urlNow: '',
            type: '',
            id: '',

            cameraType: 'back',

            initialPosition: null,
            isReadOnly: true,

            data: [
                {
                    label: 'Thành công',
                    value: "0",
                },
                {
                    label: 'Không thành công',
                    value: "1",
                }
            ],
            isOK: true,

            dataNumberPosm: [
                {
                    label: '1',
                    value: '1'
                },
                {
                    label: '2',
                    value: '2'
                },
                {
                    label: '3',
                    value: '3'
                },
                {
                    label: '4',
                    value: '4'
                },
                {
                    label: '5',
                    value: '5'
                },
                {
                    label: '6',
                    value: '6'
                },
                {
                    label: '7',
                    value: '7'
                },
                {
                    label: '8',
                    value: '8'
                },
                {
                    label: '9',
                    value: '9'
                },
                {
                    label: '10',
                    value: '10'
                },
            ],

            note: '',
            code: '',
            name: '',
            number: '',
            street: '',

            storeTypeList: [],
            storeType: undefined,

            provinceList: [],
            province: undefined,

            districtList: [],
            district: undefined,

            wardList: [],
            ward: undefined,

            storeIMGOverview: '',
            storeIMGAddress: '',

            selfie: '',

            storeData: null,

            TRANH_PEPSI_AND_7UP: [],

            STORE_FAILED: [],

            STICKER_7UP: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'STICKER_7UP',
                    code: 'HINH_KY_PXN'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'STICKER_7UP',
                    code: 'HINH_PXN_DAYDU'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'STICKER_7UP',
                    code: 'HINH_SPVB'
                }
            ],

            STICKER_PEPSI: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'STICKER_PEPSI',
                    code: 'HINH_KY_PXN'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'STICKER_PEPSI',
                    code: 'HINH_PXN_DAYDU'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'STICKER_PEPSI',
                    code: 'HINH_SPVB'
                }
            ],

            BANNER_PEPSI: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'BANNER_PEPSI',
                    code: 'HINH_KY_PXN'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'BANNER_PEPSI',
                    code: 'HINH_PXN_DAYDU'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'BANNER_PEPSI',
                    code: 'HINH_SPVB'
                }
            ],

            BANNER_7UP_TET: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'BANNER_7UP_TET',
                    code: 'HINH_KY_PXN'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'BANNER_7UP_TET',
                    code: 'HINH_PXN_DAYDU'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'BANNER_7UP_TET',
                    code: 'HINH_SPVB'
                }
            ],

            BANNER_MIRINDA: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'BANNER_MIRINDA',
                    code: 'HINH_KY_PXN'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'BANNER_MIRINDA',
                    code: 'HINH_PXN_DAYDU'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'BANNER_MIRINDA',
                    code: 'HINH_SPVB'
                }
            ],

            BANNER_TWISTER: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'BANNER_TWISTER',
                    code: 'HINH_KY_PXN'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'BANNER_TWISTER',
                    code: 'HINH_PXN_DAYDU'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'BANNER_TWISTER',
                    code: 'HINH_SPVB'
                }
            ],

            BANNER_REVIVE: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'BANNER_REVIVE',
                    code: 'HINH_KY_PXN'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'BANNER_REVIVE',
                    code: 'HINH_PXN_DAYDU'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'BANNER_REVIVE',
                    code: 'HINH_SPVB'
                }
            ],

            BANNER_OOLONG: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'BANNER_OOLONG',
                    code: 'HINH_KY_PXN'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'BANNER_OOLONG',
                    code: 'HINH_PXN_DAYDU'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'BANNER_OOLONG',
                    code: 'HINH_SPVB'
                }
            ],

            numTRANH_PEPSI_AND_7UP: 0,
            numSTICKER_7UP: 0,
            numSTICKER_PEPSI: 0,
            numBANNER_PEPSI: 0,
            numBANNER_7UP_TET: 0,
            numBANNER_MIRINDA: 0,
            numBANNER_TWISTER: 0,
            numBANNER_REVIVE: 0,
            numBANNER_OOLONG: 0
        };
    }

    componentWillMount() {
        this._getDataSetup();
    }

    componentDidMount() {
        navigator.geolocation.getCurrentPosition(
            (position) => {
                let initialPosition = JSON.stringify(position);
                var obj = JSON.parse(initialPosition);
                this.setState({ initialPosition: obj });
            },
            (error) => alert(error.message),
            { enableHighAccuracy: false, timeout: 20000, maximumAge: 3000 }
        );

        BackHandler.addEventListener('hardwareBackPress', this.handleBackPress);
    }

    componentWillUnmount() {
        BackHandler.removeEventListener('hardwareBackPress', this.handleBackPress);
    }

    handleBackPress = () => {

        const { isSave, isCamera, isMain, isPreview } = this.state;

        if (isCamera) {
            this.setState({ isCamera: false, isMain: true, isPreview: false });
        }
        else if (isPreview) {
            this.setState({ isCamera: false, isMain: true, isPreview: false });
        }
        else if (isMain) {
            if (isSave === false) {
                Alert.alert(
                    STRINGS.MessageTitleAlert, 'Bạn chưa lưu dữ liệu, bạn có chắc chắn thoát trang này, dữ liệu sẽ bị mất ?',
                    [{
                        text: STRINGS.MessageActionOK, onPress: () => {
                            this.props.navigation.navigate('Home');
                            return false;
                        }
                    },
                    { text: STRINGS.MessageActionCancel, onPress: () => console.log('Cancel Pressed') }],
                    { cancelable: false }
                );
            }
            else {
                this.props.navigation.navigate('Home');
                return false;
            }
        }

        return true;
    }

    _getDataSetup = async () => {
        // Call API
        await this.props.fetchDataGetAllProvinces()
            .then(() => setTimeout(() => {
                this.bindDataProvince()
            }, 100));
    }

    // change combobox

    _changeOptionProvince = async (id) => {

        await this.props.fetchDataGetAllDistrics(id)
            .then(() => setTimeout(() => {
                this.bindDataDistrict()
            }, 100));
    }

    _changeOptionDistrict = async (id) => {

        await this.props.fetchDataGetAllWards(id)
            .then(() => setTimeout(() => {
                this.bindDataWard()
            }, 100));
    }

    // back

    handleBack = () => {

        const { isSave } = this.state;

        if (isSave === false) {
            Alert.alert(
                STRINGS.MessageTitleAlert, 'Bạn chưa lưu dữ liệu, bạn có chắc chắn thoát trang này, dữ liệu sẽ bị mất ?',
                [{
                    text: STRINGS.MessageActionOK, onPress: () => {
                        this.props.navigation.navigate('Home');
                    }
                },
                { text: STRINGS.MessageActionCancel, onPress: () => console.log('Cancel Pressed') }],
                { cancelable: false }
            );
        }
        else {
            this.props.navigation.navigate('Home');
        }
    }

    // find store
    handleFindStore = () => {

        this.setState({ isReadOnly: true });

        const { code } = this.state;
        if (code == '') {
            Alert.alert('Lỗi', 'Vui lòng nhập mã cửa hàng');
        }
        else {
            this.props.fetchDataGetStoreByCode(code)
                .then(() => setTimeout(() => {
                    this.bindDataStore()
                }, 100));
        }
    }

    updateStore = async () => {
        this.setState({ isReadOnly: false });

        await this.props.fetchDataGetAllStoreType()
            .then(() => setTimeout(() => {
                this.bindDataStoreType()
            }, 100));
    }

    // bind data

    bindDataStoreType = () => {
        const { dataResListStoreType, error, errorMessage } = this.props;

        if (error) {
            let _mess = errorMessage + '';
            if (errorMessage == 'TypeError: Network request failed')
                _mess = STRINGS.MessageNetworkError;

            Alert.alert(
                STRINGS.MessageTitleError, _mess,
                [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
            );
            return;
        }
        else {
            if (dataResListStoreType.HasError == true) {
                Alert.alert(
                    STRINGS.MessageTitleError, dataResListStoreType.Message + '',
                    [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
                );
            } else if (dataResListStoreType.HasError == false) {

                var list = [];

                if (dataResListStoreType.Data.lenggth != 0) {
                    dataResListStoreType.Data.forEach(element => {
                        list.push({
                            "label": element.Name,
                            "value": element.Id
                        });
                    });

                    this.setState({ storeTypeList: list });
                }
            }
        }
    }

    bindDataProvince = () => {
        const { dataResListProvinces, error, errorMessage } = this.props;

        if (error) {
            let _mess = errorMessage + '';
            if (errorMessage == 'TypeError: Network request failed')
                _mess = STRINGS.MessageNetworkError;

            Alert.alert(
                STRINGS.MessageTitleError, _mess,
                [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
            );
            return;
        }
        else {
            if (dataResListProvinces.HasError == true) {
                Alert.alert(
                    STRINGS.MessageTitleError, dataResListProvinces.Message + '',
                    [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
                );
            } else if (dataResListProvinces.HasError == false) {

                var list = [];

                if (dataResListProvinces.Data.lenggth != 0) {
                    dataResListProvinces.Data.forEach(element => {
                        list.push({
                            "label": element.Name,
                            "value": element.Id
                        });
                    });

                    this.setState({ provinceList: list });
                }
            }
        }
    }

    bindDataDistrict = () => {
        const { dataResListDistricts, error, errorMessage } = this.props;

        if (error) {
            let _mess = errorMessage + '';
            if (errorMessage == 'TypeError: Network request failed')
                _mess = STRINGS.MessageNetworkError;

            Alert.alert(
                STRINGS.MessageTitleError, _mess,
                [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
            );
            return;
        }
        else {
            if (dataResListDistricts.HasError == true) {
                Alert.alert(
                    STRINGS.MessageTitleError, dataResListDistricts.Message + '',
                    [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
                );
            } else if (dataResListDistricts.HasError == false) {

                var list = [];

                if (dataResListDistricts.Data.lenggth != 0) {
                    dataResListDistricts.Data.forEach(element => {
                        list.push({
                            "label": element.Name,
                            "value": element.Id
                        });
                    });

                    this.setState({ districtList: list });
                }
            }
        }
    }

    bindDataWard = () => {
        const { dataResListWards, error, errorMessage } = this.props;

        if (error) {
            let _mess = errorMessage + '';
            if (errorMessage == 'TypeError: Network request failed')
                _mess = STRINGS.MessageNetworkError;

            Alert.alert(
                STRINGS.MessageTitleError, _mess,
                [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
            );
            return;
        }
        else {
            if (dataResListWards.HasError == true) {
                Alert.alert(
                    STRINGS.MessageTitleError, dataResListWards.Message + '',
                    [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
                );
            } else if (dataResListWards.HasError == false) {

                var list = [];

                if (dataResListWards.Data.lenggth != 0) {
                    dataResListWards.Data.forEach(element => {
                        list.push({
                            "label": element.Name,
                            "value": element.Id
                        });
                    });

                    this.setState({ wardList: list });
                }
            }
        }
    }

    bindDataStore = () => {
        const { dataResStore, error, errorMessage } = this.props;

        if (error) {
            let _mess = errorMessage + '';
            if (errorMessage == 'TypeError: Network request failed')
                _mess = STRINGS.MessageNetworkError;

            Alert.alert(
                STRINGS.MessageTitleError, _mess,
                [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
            );
            return;
        }
        else {
            if (dataResStore.HasError == true) {
                Alert.alert(
                    STRINGS.MessageTitleError, dataResStore.Message + '',
                    [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
                );
            } else if (dataResStore.HasError == false) {

                this.setState({
                    storeData: dataResStore.Data,
                    name: dataResStore.Data.Name, number: dataResStore.Data.HouseNumber,
                    street: dataResStore.Data.StreetNames, province: dataResStore.Data.ProvinceId,
                    district: dataResStore.Data.DistrictId, ward: dataResStore.Data.WardId
                });

            }
        }
    }

    // add img

    addTRANH_PEPSI_AND_7UP = () => {
        const items = this.state.TRANH_PEPSI_AND_7UP;

        var item = {
            id: items.length,
            title: '...',
            url: '',
            type: 'TRANH_PEPSI_AND_7UP',
            code: ''
        };
        items.push(item);

        this.setState({ TRANH_PEPSI_AND_7UP: items });

        // this.forceUpdate();

        this.handleTakePhoto('TRANH_PEPSI_AND_7UP', item.id);
    }

    addSTICKER_7UP = () => {
        const items = this.state.STICKER_7UP;

        var item = {
            id: items.length,
            title: '...',
            url: '',
            type: 'STICKER_7UP',
            code: ''
        };
        items.push(item);

        this.setState({ STICKER_7UP: items });

        this.forceUpdate();

        this.handleTakePhoto('STICKER_7UP', item.id);
    }

    addSTICKER_PEPSI = () => {
        const items = this.state.STICKER_PEPSI;

        var item = {
            id: items.length,
            title: '...',
            url: '',
            type: 'STICKER_PEPSI',
            code: ''
        };
        items.push(item);

        this.setState({ STICKER_PEPSI: items });

        this.forceUpdate();

        this.handleTakePhoto('STICKER_PEPSI', item.id);
    }

    addBANNER_PEPSI = () => {
        const items = this.state.BANNER_PEPSI;

        var item = {
            id: items.length,
            title: '...',
            url: '',
            type: 'BANNER_PEPSI',
            code: ''
        };
        items.push(item);

        this.setState({ BANNER_PEPSI: items });

        this.forceUpdate();

        this.handleTakePhoto('BANNER_PEPSI', item.id);
    }

    addBANNER_7UP_TET = () => {
        const items = this.state.BANNER_7UP_TET;

        var item = {
            id: items.length,
            title: '...',
            url: '',
            type: 'BANNER_7UP_TET',
            code: ''
        };
        items.push(item);

        this.setState({ BANNER_7UP_TET: items });

        this.forceUpdate();

        this.handleTakePhoto('BANNER_7UP_TET', item.id);
    }

    addBANNER_MIRINDA = () => {
        const items = this.state.BANNER_MIRINDA;

        var item = {
            id: items.length,
            title: '...',
            url: '',
            type: 'BANNER_MIRINDA',
            code: ''
        };
        items.push(item);

        this.setState({ BANNER_MIRINDA: items });

        this.forceUpdate();

        this.handleTakePhoto('BANNER_MIRINDA', item.id);
    }

    addBANNER_TWISTER = () => {
        const items = this.state.BANNER_TWISTER;

        var item = {
            id: items.length,
            title: '...',
            url: '',
            type: 'BANNER_TWISTER',
            code: ''
        };
        items.push(item);

        this.setState({ BANNER_TWISTER: items });

        this.forceUpdate();

        this.handleTakePhoto('BANNER_TWISTER', item.id);
    }

    addBANNER_REVIVE = () => {
        const items = this.state.BANNER_REVIVE;

        var item = {
            id: items.length,
            title: '...',
            url: '',
            type: 'BANNER_REVIVE',
            code: ''
        };
        items.push(item);

        this.setState({ BANNER_REVIVE: items });

        this.forceUpdate();

        this.handleTakePhoto('BANNER_REVIVE', item.id);
    }

    addBANNER_OOLONG = () => {
        const items = this.state.BANNER_OOLONG;

        var item = {
            id: items.length,
            title: '...',
            url: '',
            type: 'BANNER_OOLONG',
            code: ''
        };
        items.push(item);

        this.setState({ BANNER_OOLONG: items });

        this.forceUpdate();

        this.handleTakePhoto('BANNER_OOLONG', item.id);
    }

    addSTORE_FAILED = () => {
        const items = this.state.STORE_FAILED;

        var item = {
            id: items.length,
            title: '...',
            url: '',
            type: 'STORE_FAILED',
            code: ''
        };
        items.push(item);

        this.setState({ STORE_FAILED: items });

        // this.forceUpdate();

        this.handleTakePhoto('STORE_FAILED', item.id);
    }

    // push info to server

    _pushInfoToServer = async () => {

        const { dataResUser } = this.props;

        const { name, street, number, province,
            district, ward, note, storeData, initialPosition, isOK } = this.state;

        var storeID = '';
        if (storeData !== null) {
            storeID = storeData.Id;
        }

        let item = {
            Token: dataResUser.Data.Token,
            Id: dataResUser.Data.Id,
            MaterStoreName: name,
            HouseNumber: number,
            StreetNames: street,
            ProvinceId: province,
            DistrictId: district,
            WardId: ward,
            Lat: initialPosition ? initialPosition.coords.latitude : '',
            Lng: initialPosition ? initialPosition.coords.longitude : '',
            Note: note,
            Region: '',
            MasterStoreId: storeID,
            Date: moment().format('DD/MM/YYYY'),
            StoreStatus: isOK ? true : false
        };

        // Call API
        await this.props.fetchPushInfoToServer(item)
            .then(() => setTimeout(() => {
                this.bindDataPushToServer()
            }, 100));
    }

    bindDataPushToServer() {

        const { PushInfoData, PushInfoError, PushInfoErrorMessage } = this.props;

        if (PushInfoError) {
            let _mess = PushInfoErrorMessage + '';
            if (PushInfoErrorMessage == 'TypeError: Network request failed')
                _mess = STRINGS.MessageNetworkError;

            Alert.alert(
                STRINGS.MessageTitleError, _mess,
                [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
            );
            return;
        }
        else {
            if (PushInfoData.HasError == true) {
                Alert.alert(
                    STRINGS.MessageTitleError, PushInfoData.Message + '',
                    [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
                );
            } else if (PushInfoData.HasError == false) {

                this._storeDataToLocal(PushInfoData.Data.Id);
            }
        }
    }

    // save data to local

    _storeDataToLocal = async (trackId) => {

        const { dataResUser } = this.props;

        const { storeData, numBANNER_7UP_TET, numBANNER_MIRINDA, numBANNER_OOLONG,
            numBANNER_PEPSI, numBANNER_REVIVE, numBANNER_TWISTER,
            numSTICKER_7UP, numSTICKER_PEPSI, numTRANH_PEPSI_AND_7UP } = this.state;

        const {
            storeIMGAddress, storeIMGOverview, selfie,
            STICKER_7UP, STICKER_PEPSI, BANNER_PEPSI,
            BANNER_7UP_TET, BANNER_MIRINDA, BANNER_OOLONG,
            BANNER_REVIVE, BANNER_TWISTER, TRANH_PEPSI_AND_7UP,
            STORE_FAILED } = this.state;

        const { name, code } = this.state;

        try {

            let item = {
                Name: name,
                Code: code,
                Date: moment().format('DD/MM/YYYY'),
                POSM: []
            };

            let listIMGFinal = [];
            let listIMG = [];
            listIMG.push.apply(listIMG, STICKER_7UP);
            listIMG.push.apply(listIMG, STICKER_PEPSI);
            listIMG.push.apply(listIMG, BANNER_PEPSI);
            listIMG.push.apply(listIMG, BANNER_7UP_TET);
            listIMG.push.apply(listIMG, BANNER_MIRINDA);
            listIMG.push.apply(listIMG, BANNER_OOLONG);
            listIMG.push.apply(listIMG, BANNER_REVIVE);
            listIMG.push.apply(listIMG, BANNER_TWISTER);
            listIMG.push.apply(listIMG, TRANH_PEPSI_AND_7UP);
            listIMG.push.apply(listIMG, STORE_FAILED);

            // Add to list final
            if (selfie !== '') {
                listIMGFinal.push(
                    {
                        Id: dataResUser.Data.Id,
                        Code: 'SELFIE',
                        Code2: '',
                        Date: moment().format('DD/MM/YYYY'),
                        // MasterStoreId: storeData.Id,
                        Token: dataResUser.Data.Token,
                        TrackSessionId: trackId,
                        PosmNumber: 0,
                        Photo: {
                            uri: selfie,
                            type: 'image/jpeg',
                            name: 'SELFIE'
                        },
                        IsSubmit: false
                    }
                );
            }

            if (storeIMGAddress !== '') {
                listIMGFinal.push(
                    {
                        Id: dataResUser.Data.Id,
                        Code: 'DEFAULT',
                        Code2: '',
                        Date: moment().format('DD/MM/YYYY'),
                        // MasterStoreId: storeData.Id,
                        Token: dataResUser.Data.Token,
                        TrackSessionId: trackId,
                        PosmNumber: 0,
                        Photo: {
                            uri: storeIMGAddress,
                            type: 'image/jpeg',
                            name: 'DEFAULT'
                        },
                        IsSubmit: false
                    }
                );
            }

            if (storeIMGOverview !== '') {
                listIMGFinal.push(
                    {
                        Id: dataResUser.Data.Id,
                        Code: 'DEFAULT',
                        Code2: '',
                        Date: moment().format('DD/MM/YYYY'),
                        // MasterStoreId: storeData.Id,
                        Token: dataResUser.Data.Token,
                        TrackSessionId: trackId,
                        PosmNumber: 0,
                        Photo: {
                            uri: storeIMGOverview,
                            type: 'image/jpeg',
                            name: 'DEFAULT'
                        },
                        IsSubmit: false
                    }
                );
            }

            listIMG.forEach(element => {
                if (element.url !== '' && element.url !== null) {

                    let posmNum = 0;

                    switch (element.type) {
                        case 'TRANH_PEPSI_AND_7UP':
                            {
                                posmNum = numTRANH_PEPSI_AND_7UP;
                                break;
                            }
                        case 'STICKER_7UP':
                            {
                                posmNum = numSTICKER_7UP;
                                break;
                            }
                        case 'STICKER_PEPSI':
                            {
                                posmNum = numSTICKER_PEPSI;
                                break;
                            }
                        case 'BANNER_PEPSI':
                            {
                                posmNum = numBANNER_PEPSI;
                                break;
                            }
                        case 'BANNER_7UP_TET':
                            {
                                posmNum = numBANNER_7UP_TET;
                                break;
                            }
                        case 'BANNER_MIRINDA':
                            {
                                posmNum = numBANNER_MIRINDA;
                                break;
                            }
                        case 'BANNER_TWISTER':
                            {
                                posmNum = numBANNER_TWISTER;
                                break;
                            }
                        case 'BANNER_REVIVE':
                            {
                                posmNum = numBANNER_REVIVE;
                                break;
                            }
                        case 'BANNER_OOLONG':
                            {
                                posmNum = numBANNER_OOLONG;
                                break;
                            }
                    }

                    listIMGFinal.push(
                        {
                            Id: dataResUser.Data.Id,
                            Code: element.type,
                            Code2: element.code,
                            Date: moment().format('DD/MM/YYYY'),
                            // MasterStoreId: storeData.Id,
                            Token: dataResUser.Data.Token,
                            TrackSessionId: trackId,
                            PosmNumber: posmNum,
                            Photo: {
                                uri: element.url,
                                type: 'image/jpeg',
                                name: element.type,
                            },
                            IsSubmit: false
                        }
                    );
                }
            });

            item.POSM = listIMGFinal;

            const _data = await AsyncStorage.getItem('DATA_SSC');

            if (_data != null) {
                let dataParse = JSON.parse(_data);
                dataParse.push(item);
                await AsyncStorage.setItem('DATA_SSC', JSON.stringify(dataParse));
            }
            else {
                var _newData = [];
                _newData.push(item);
                await AsyncStorage.setItem('DATA_SSC', JSON.stringify(_newData));
            }

            Alert.alert('Thông báo', 'Lưu dữ liệu thành công');

            this.setState({ isSave: true });
        } catch (error) {
            Alert.alert('Lỗi', error + '');
        }
    }

    // remove file

    removeFile = (filepath) => {
        RNFS.exists(filepath)
            .then((result) => {

                if (result) {
                    return RNFS.unlink(filepath)
                        .then(() => {
                            // Alert.alert('FILE DELETED');
                        })
                        .catch((err) => {
                            console.log(err.message);
                        });
                }

            })
            .catch((err) => {
                console.log(err.message);
            });
    }

    // camera

    takePicture = async function () {
        if (this.camera) {
            const options = { quality: 1, base64: false, width: 1080, height: 1920, fixOrientation: true, forceUpOrientation: true };
            const data = await this.camera.takePictureAsync(options)

            this.setState({ urlNow: data.uri, isCamera: false, isMain: false, isPreview: true });
        }
    };

    okPicture = async function () {
        const { urlNow, type, id } = this.state;

        if (type == 'SELFIE') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {
            //     console.log('photone', data)
            //     this.setState({
            //         selfie: data,
            //     });

            //     this._pushInfoToServer();
            // });

            this.setState({
                selfie: urlNow,
            });

            this._pushInfoToServer();
        }
        else if (type == 'DEFAULT_1') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {
            //     console.log('photone', data)
            //     this.setState({
            //         storeIMGOverview: data,
            //     });
            // });

            this.setState({
                storeIMGOverview: urlNow,
            });

            // this.removeFile(urlNow);
        }
        else if (type == 'DEFAULT_2') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {
            //     console.log('photone', data)
            //     this.setState({
            //         storeIMGAddress: data,
            //     });
            // });

            this.setState({
                storeIMGAddress: urlNow,
            });

            // this.removeFile(urlNow);
        }
        else if (type == 'STICKER_7UP') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {

            //     const items = this.state.STICKER_7UP;
            //     items[id].url = data;

            //     this.forceUpdate();
            // });

            const items = this.state.STICKER_7UP;
            items[id].url = urlNow;

            this.forceUpdate();

            // this.removeFile(urlNow);
        } else if (type == 'STICKER_PEPSI') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {

            //     const items = this.state.STICKER_PEPSI;
            //     items[id].url = data;

            //     this.forceUpdate();
            // });

            const items = this.state.STICKER_PEPSI;
            items[id].url = urlNow;

            this.forceUpdate();

            // this.removeFile(urlNow);
        } else if (type == 'BANNER_PEPSI') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {

            //     const items = this.state.BANNER_PEPSI;
            //     items[id].url = data;

            //     this.forceUpdate();
            // });

            const items = this.state.BANNER_PEPSI;
            items[id].url = urlNow;

            this.forceUpdate();

            // this.removeFile(urlNow);
        }
        else if (type == 'BANNER_7UP_TET') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {

            //     const items = this.state.BANNER_7UP_TET;
            //     items[id].url = data;

            //     this.forceUpdate();
            // });

            const items = this.state.BANNER_7UP_TET;
            items[id].url = urlNow;

            this.forceUpdate();

            // this.removeFile(urlNow);
        }
        else if (type == 'BANNER_MIRINDA') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {

            //     const items = this.state.BANNER_MIRINDA;
            //     items[id].url = data;

            //     this.forceUpdate();
            // });

            const items = this.state.BANNER_MIRINDA;
            items[id].url = urlNow;

            this.forceUpdate();

            // this.removeFile(urlNow);
        }
        else if (type == 'BANNER_TWISTER') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {

            //     const items = this.state.BANNER_TWISTER;
            //     items[id].url = data;

            //     this.forceUpdate();
            // });

            const items = this.state.BANNER_TWISTER;
            items[id].url = urlNow;

            this.forceUpdate();

            // this.removeFile(urlNow);
        }
        else if (type == 'BANNER_REVIVE') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {

            //     const items = this.state.BANNER_REVIVE;
            //     items[id].url = data;

            //     this.forceUpdate();
            // });

            const items = this.state.BANNER_REVIVE;
            items[id].url = urlNow;

            this.forceUpdate();

            // this.removeFile(urlNow);
        }
        else if (type == 'BANNER_OOLONG') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {

            //     const items = this.state.BANNER_OOLONG;
            //     items[id].url = data;

            //     this.forceUpdate();
            // });

            const items = this.state.BANNER_OOLONG;
            items[id].url = urlNow;

            this.forceUpdate();

            // this.removeFile(urlNow);
        }
        else if (type == 'TRANH_PEPSI_AND_7UP') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {

            //     const items = this.state.TRANH_PEPSI_AND_7UP;
            //     items[id].url = data;

            //     this.forceUpdate();
            // });

            const items = this.state.TRANH_PEPSI_AND_7UP;
            items[id].url = urlNow;

            this.forceUpdate();

            // this.removeFile(urlNow);
        }
        else if (type == 'STORE_FAILED') {

            // CameraRoll.saveToCameraRoll(urlNow, 'photo').then((data) => {

            //     const items = this.state.STORE_FAILED;
            //     items[id].url = data;

            //     this.forceUpdate();
            // });

            const items = this.state.STORE_FAILED;
            items[id].url = urlNow;

            this.forceUpdate();

            // this.removeFile(urlNow);
        }

        this.setState({ isCamera: false, isMain: true, isPreview: false });
    };

    takeSelfiePhoto = () => {

        const { storeData, isReadOnly } = this.state;

        if (storeData === null && isReadOnly) {
            Alert.alert('Lỗi', 'Vui lòng chọn cửa hàng');
            return;
        }

        this.setState({ cameraType: 'front' });

        this.handleTakePhotoSelfie('SELFIE');
    }

    handleTakePhoto(type, id) {
        this.setState({ isCamera: true, isMain: false, isPreview: false, type: type, id: id });
    }

    handleTakePhotoSelfie(type) {
        this.setState({ isCamera: true, isMain: false, isPreview: false, type: type });
    }

    // option change

    handleOptionChange(value) {
        this.setState({ isOK: false });
        value.forEach(element => {
            if (element.value === "0" && element.selected) {
                this.setState({ isOK: true });
                return;
            }
        });
    }

    changeCameraType = () => {
        const { cameraType } = this.state;

        if (cameraType === 'back') {
            this.setState({ cameraType: 'front' });
        }
        else {
            this.setState({ cameraType: 'back' });
        }
    }

    // render item

    renderImageItemStore(title, url, type) {
        return (
            <TouchableOpacity onPress={() => this.handleTakePhoto(type)}>
                <View style={styles.imgContainer}>
                    <View style={styles.card}>
                        <Image
                            source={{ uri: url }}
                            style={styles.cardImage}
                            resizeMode="cover"
                        />
                    </View>
                    <Text style={{ textAlign: 'center' }}>{title}</Text>
                </View>
            </TouchableOpacity>
        );
    }

    renderImageItemPOSM(item) {
        const { title, url, type, id } = item;

        return (
            <TouchableOpacity onPress={() => this.handleTakePhoto(type, id)}>
                <View style={styles.imgContainer}>
                    <View style={styles.card2}>
                        <Image
                            source={{ uri: url }}
                            style={styles.cardImage}
                            resizeMode="cover"
                        />
                    </View>
                    <Text style={{ textAlign: 'center' }}>{title}</Text>
                </View>
            </TouchableOpacity>
        );
    }

    // render

    renderNotOK() {

        const { STORE_FAILED } = this.state;

        return (
            <View>
                <View style={styles.line} />

                <View style={styles.centerContainer}>
                    <Text style={styles.itemSubTitle} uppercase>{'Hình ảnh không đạt'}</Text>
                </View>

                <View style={styles.rowContainer2}>
                    <ScrollView
                        horizontal={true}
                        showsHorizontalScrollIndicator={false}
                        contentContainerStyle={{
                            height: CARD_HEIGHT + 10
                        }}
                        style={{ padding: 0 }}>
                        {STORE_FAILED.map((item, index) => {
                            return (
                                this.renderImageItemPOSM(item)
                            );
                        })}
                        <MainButton
                            style={styles.button2}
                            title={'Thêm'}
                            onPress={this.addSTORE_FAILED} />
                    </ScrollView>
                </View>
            </View>
        );
    }

    renderOK() {
        const {
            storeIMGAddress, storeIMGOverview,
            STICKER_7UP, STICKER_PEPSI, BANNER_PEPSI,
            BANNER_7UP_TET, BANNER_MIRINDA, BANNER_OOLONG,
            BANNER_REVIVE, BANNER_TWISTER, TRANH_PEPSI_AND_7UP,
            dataNumberPosm,
            numBANNER_7UP_TET, numBANNER_MIRINDA, numBANNER_OOLONG,
            numBANNER_PEPSI, numBANNER_REVIVE, numBANNER_TWISTER,
            numSTICKER_7UP, numSTICKER_PEPSI, numTRANH_PEPSI_AND_7UP } = this.state;

        return (
            <View>
                {/* Store img*/}

                <View style={styles.line} />

                <View style={styles.centerContainer}>
                    <Text style={styles.itemSubTitle} uppercase>{STRINGS.POSMDetailSubTitle1}</Text>
                </View>

                <View style={styles.rowContainer}>
                    {
                        this.renderImageItemStore('H. Tổng quan', storeIMGOverview, 'DEFAULT_1')
                    }
                    {
                        this.renderImageItemStore('H. Địa chỉ', storeIMGAddress, 'DEFAULT_2')
                    }
                </View>

                {/* Tranh Pepsi & 7Up */}

                <View style={styles.line} />

                <View style={styles.centerContainer}>
                    <Text style={styles.itemSubTitle} uppercase>{'Tranh Pepsi & 7Up'}</Text>
                </View>

                <View style={styles.rowContainer2}>
                    <ScrollView
                        horizontal={true}
                        showsHorizontalScrollIndicator={false}
                        contentContainerStyle={{
                            height: CARD_HEIGHT + 10
                        }}
                        style={{ padding: 0 }}>
                        {TRANH_PEPSI_AND_7UP.map((item, index) => {
                            return (
                                this.renderImageItemPOSM(item)
                            );
                        })}
                        <MainButton
                            style={styles.button2}
                            title={'Thêm'}
                            onPress={this.addTRANH_PEPSI_AND_7UP} />
                    </ScrollView>
                </View>

                <View style={styles.rowContainer}>
                    <View style={styles.leftItem}>
                        <Text style={styles.title}>{'Số lượng'}</Text>
                    </View>
                    <View style={styles.rightItem}>
                        <RNPickerSelect
                            placeholder={{
                                label: 'Chọn...',
                                value: null,
                            }}
                            items={dataNumberPosm}
                            onValueChange={(value) => {
                                this.setState({
                                    numTRANH_PEPSI_AND_7UP: value,
                                });
                            }}
                            hideDoneBar={true}
                            style={{ ...pickerSelectStyles }}
                            value={numTRANH_PEPSI_AND_7UP}
                        />
                    </View>
                </View>

                {/* Sticker 7Up  */}

                <View style={styles.line} />

                <View style={styles.centerContainer}>
                    <Text style={styles.itemSubTitle} uppercase>{'Sticker 7Up'}</Text>
                </View>

                <View style={styles.rowContainer2}>
                    <ScrollView
                        horizontal={true}
                        showsHorizontalScrollIndicator={false}
                        contentContainerStyle={{
                            height: CARD_HEIGHT + 10
                        }}
                        style={{ padding: 0 }}>
                        {STICKER_7UP.map((item, index) => {
                            return (
                                this.renderImageItemPOSM(item)
                            );
                        })}
                        <MainButton
                            style={styles.button2}
                            title={'Thêm'}
                            onPress={this.addSTICKER_7UP} />
                    </ScrollView>
                </View>

                <View style={styles.rowContainer}>
                    <View style={styles.leftItem}>
                        <Text style={styles.title}>{'Số lượng'}</Text>
                    </View>
                    <View style={styles.rightItem}>
                        <RNPickerSelect
                            placeholder={{
                                label: 'Chọn...',
                                value: null,
                            }}
                            items={dataNumberPosm}
                            onValueChange={(value) => {
                                this.setState({
                                    numSTICKER_7UP: value,
                                });
                            }}
                            hideDoneBar={true}
                            style={{ ...pickerSelectStyles }}
                            value={numSTICKER_7UP}
                        />
                    </View>
                </View>

                {/* Sticker Pepsi */}

                <View style={styles.line} />

                <View style={styles.centerContainer}>
                    <Text style={styles.itemSubTitle} uppercase>{'Sticker Pepsi'}</Text>
                </View>

                <View style={styles.rowContainer2}>
                    <ScrollView
                        horizontal={true}
                        showsHorizontalScrollIndicator={false}
                        contentContainerStyle={{
                            height: CARD_HEIGHT + 10
                        }}
                        style={{ padding: 0 }}>
                        {STICKER_PEPSI.map((item, index) => {
                            return (
                                this.renderImageItemPOSM(item)
                            );
                        })}
                        <MainButton
                            style={styles.button2}
                            title={'Thêm'}
                            onPress={this.addSTICKER_PEPSI} />
                    </ScrollView>
                </View>

                <View style={styles.rowContainer}>
                    <View style={styles.leftItem}>
                        <Text style={styles.title}>{'Số lượng'}</Text>
                    </View>
                    <View style={styles.rightItem}>
                        <RNPickerSelect
                            placeholder={{
                                label: 'Chọn...',
                                value: null,
                            }}
                            items={dataNumberPosm}
                            onValueChange={(value) => {
                                this.setState({
                                    numSTICKER_PEPSI: value,
                                });
                            }}
                            hideDoneBar={true}
                            style={{ ...pickerSelectStyles }}
                            value={numSTICKER_PEPSI}
                        />
                    </View>
                </View>

                {/* Banner  Pepsi */}

                <View style={styles.line} />

                <View style={styles.centerContainer}>
                    <Text style={styles.itemSubTitle} uppercase>{'Banner  Pepsi'}</Text>
                </View>

                <View style={styles.rowContainer2}>
                    <ScrollView
                        horizontal={true}
                        showsHorizontalScrollIndicator={false}
                        contentContainerStyle={{
                            height: CARD_HEIGHT + 10
                        }}
                        style={{ padding: 0 }}>
                        {BANNER_PEPSI.map((item, index) => {
                            return (
                                this.renderImageItemPOSM(item)
                            );
                        })}
                        <MainButton
                            style={styles.button2}
                            title={'Thêm'}
                            onPress={this.addBANNER_PEPSI} />
                    </ScrollView>
                </View>

                <View style={styles.rowContainer}>
                    <View style={styles.leftItem}>
                        <Text style={styles.title}>{'Số lượng'}</Text>
                    </View>
                    <View style={styles.rightItem}>
                        <RNPickerSelect
                            placeholder={{
                                label: 'Chọn...',
                                value: null,
                            }}
                            items={dataNumberPosm}
                            onValueChange={(value) => {
                                this.setState({
                                    numBANNER_PEPSI: value,
                                });
                            }}
                            hideDoneBar={true}
                            style={{ ...pickerSelectStyles }}
                            value={numBANNER_PEPSI}
                        />
                    </View>
                </View>

                {/* Banner 7Up Tết */}

                <View style={styles.line} />

                <View style={styles.centerContainer}>
                    <Text style={styles.itemSubTitle} uppercase>{'Banner 7Up Tết'}</Text>
                </View>

                <View style={styles.rowContainer2}>
                    <ScrollView
                        horizontal={true}
                        showsHorizontalScrollIndicator={false}
                        contentContainerStyle={{
                            height: CARD_HEIGHT + 10
                        }}
                        style={{ padding: 0 }}>
                        {BANNER_7UP_TET.map((item, index) => {
                            return (
                                this.renderImageItemPOSM(item)
                            );
                        })}
                        <MainButton
                            style={styles.button2}
                            title={'Thêm'}
                            onPress={this.addBANNER_7UP_TET} />
                    </ScrollView>
                </View>

                <View style={styles.rowContainer}>
                    <View style={styles.leftItem}>
                        <Text style={styles.title}>{'Số lượng'}</Text>
                    </View>
                    <View style={styles.rightItem}>
                        <RNPickerSelect
                            placeholder={{
                                label: 'Chọn...',
                                value: null,
                            }}
                            items={dataNumberPosm}
                            onValueChange={(value) => {
                                this.setState({
                                    numBANNER_7UP_TET: value,
                                });
                            }}
                            hideDoneBar={true}
                            style={{ ...pickerSelectStyles }}
                            value={numBANNER_7UP_TET}
                        />
                    </View>
                </View>

                {/* Banner Mirinda */}

                <View style={styles.line} />

                <View style={styles.centerContainer}>
                    <Text style={styles.itemSubTitle} uppercase>{'Banner Mirinda'}</Text>
                </View>

                <View style={styles.rowContainer2}>
                    <ScrollView
                        horizontal={true}
                        showsHorizontalScrollIndicator={false}
                        contentContainerStyle={{
                            height: CARD_HEIGHT + 10
                        }}
                        style={{ padding: 0 }}>
                        {BANNER_MIRINDA.map((item, index) => {
                            return (
                                this.renderImageItemPOSM(item)
                            );
                        })}
                        <MainButton
                            style={styles.button2}
                            title={'Thêm'}
                            onPress={this.addBANNER_MIRINDA} />
                    </ScrollView>
                </View>

                <View style={styles.rowContainer}>
                    <View style={styles.leftItem}>
                        <Text style={styles.title}>{'Số lượng'}</Text>
                    </View>
                    <View style={styles.rightItem}>
                        <RNPickerSelect
                            placeholder={{
                                label: 'Chọn...',
                                value: null,
                            }}
                            items={dataNumberPosm}
                            onValueChange={(value) => {
                                this.setState({
                                    numBANNER_MIRINDA: value,
                                });
                            }}
                            hideDoneBar={true}
                            style={{ ...pickerSelectStyles }}
                            value={numBANNER_MIRINDA}
                        />
                    </View>
                </View>

                {/* Banner  Twister */}

                <View style={styles.line} />

                <View style={styles.centerContainer}>
                    <Text style={styles.itemSubTitle} uppercase>{'Banner  Twister'}</Text>
                </View>

                <View style={styles.rowContainer2}>
                    <ScrollView
                        horizontal={true}
                        showsHorizontalScrollIndicator={false}
                        contentContainerStyle={{
                            height: CARD_HEIGHT + 10
                        }}
                        style={{ padding: 0 }}>
                        {BANNER_TWISTER.map((item, index) => {
                            return (
                                this.renderImageItemPOSM(item)
                            );
                        })}
                        <MainButton
                            style={styles.button2}
                            title={'Thêm'}
                            onPress={this.addBANNER_TWISTER} />
                    </ScrollView>
                </View>

                <View style={styles.rowContainer}>
                    <View style={styles.leftItem}>
                        <Text style={styles.title}>{'Số lượng'}</Text>
                    </View>
                    <View style={styles.rightItem}>
                        <RNPickerSelect
                            placeholder={{
                                label: 'Chọn...',
                                value: null,
                            }}
                            items={dataNumberPosm}
                            onValueChange={(value) => {
                                this.setState({
                                    numBANNER_TWISTER: value,
                                });
                            }}
                            hideDoneBar={true}
                            style={{ ...pickerSelectStyles }}
                            value={numBANNER_TWISTER}
                        />
                    </View>
                </View>

                {/* Banner  Revive */}

                <View style={styles.line} />

                <View style={styles.centerContainer}>
                    <Text style={styles.itemSubTitle} uppercase>{'Banner  Revive'}</Text>
                </View>

                <View style={styles.rowContainer2}>
                    <ScrollView
                        horizontal={true}
                        showsHorizontalScrollIndicator={false}
                        contentContainerStyle={{
                            height: CARD_HEIGHT + 10
                        }}
                        style={{ padding: 0 }}>
                        {BANNER_REVIVE.map((item, index) => {
                            return (
                                this.renderImageItemPOSM(item)
                            );
                        })}
                        <MainButton
                            style={styles.button2}
                            title={'Thêm'}
                            onPress={this.addBANNER_REVIVE} />
                    </ScrollView>
                </View>

                <View style={styles.rowContainer}>
                    <View style={styles.leftItem}>
                        <Text style={styles.title}>{'Số lượng'}</Text>
                    </View>
                    <View style={styles.rightItem}>
                        <RNPickerSelect
                            placeholder={{
                                label: 'Chọn...',
                                value: null,
                            }}
                            items={dataNumberPosm}
                            onValueChange={(value) => {
                                this.setState({
                                    numBANNER_REVIVE: value,
                                });
                            }}
                            hideDoneBar={true}
                            style={{ ...pickerSelectStyles }}
                            value={numBANNER_REVIVE}
                        />
                    </View>
                </View>

                {/* Banner  Oolong */}

                <View style={styles.line} />

                <View style={styles.centerContainer}>
                    <Text style={styles.itemSubTitle} uppercase>{'Banner  Oolong'}</Text>
                </View>

                <View style={styles.rowContainer2}>
                    <ScrollView
                        horizontal={true}
                        showsHorizontalScrollIndicator={false}
                        contentContainerStyle={{
                            height: CARD_HEIGHT + 10
                        }}
                        style={{ padding: 0 }}>
                        {BANNER_OOLONG.map((item, index) => {
                            return (
                                this.renderImageItemPOSM(item)
                            );
                        })}
                        <MainButton
                            style={styles.button2}
                            title={'Thêm'}
                            onPress={this.addBANNER_OOLONG} />
                    </ScrollView>
                </View>

                <View style={styles.rowContainer}>
                    <View style={styles.leftItem}>
                        <Text style={styles.title}>{'Số lượng'}</Text>
                    </View>
                    <View style={styles.rightItem}>
                        <RNPickerSelect
                            placeholder={{
                                label: 'Chọn...',
                                value: null,
                            }}
                            items={dataNumberPosm}
                            onValueChange={(value) => {
                                this.setState({
                                    numBANNER_OOLONG: value,
                                });
                            }}
                            hideDoneBar={true}
                            style={{ ...pickerSelectStyles }}
                            value={numBANNER_OOLONG}
                        />
                    </View>
                </View>
            </View>
        );
    }

    renderMain() {

        const { isLoading } = this.props;

        const { storeTypeList, storeType,
            provinceList, province,
            districtList, district,
            wardList, ward, note,
            code, name, street, number, isReadOnly,
            initialPosition, isOK } = this.state;

        return (
            <View
                style={styles.container}>
                <MainHeader
                    onPress={() => this.handleBack()}
                    hasLeft={true}
                    title={STRINGS.POSMDetailTitle} />
                <Spinner visible={isLoading} />
                <View
                    padder
                    style={styles.subContainer}>

                    <ScrollView
                        horizontal={false}
                        showsHorizontalScrollIndicator={false}
                        contentContainerStyle={{
                            marginBottom: 50,
                            marginTop: 10,
                            paddingBottom: 50
                        }}
                        style={{ padding: 0 }}>

                        {/* Store Code */}

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreCode}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <Item regular style={styles.item}>
                                    <Input
                                        style={styles.input}
                                        onChangeText={text => this.setState({ code: text })}>
                                        {code}
                                    </Input>
                                </Item>
                            </View>
                        </View>

                        {/* Find Store */}

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                            </View>
                            <View style={styles.rightItem}>
                                <MainButton
                                    style={styles.button}
                                    title={'Tìm cửa hàng'}
                                    onPress={this.handleFindStore} />
                            </View>
                        </View>

                        {/* Store Name */}

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreName}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <Item regular style={styles.item}>
                                    <Input
                                        disabled={isReadOnly}
                                        style={styles.input}
                                        onChangeText={text => this.setState({ name: text })}>
                                        {name}
                                    </Input>
                                </Item>
                            </View>
                        </View>

                        {/* Store Number */}

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreNumber}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <Item regular style={styles.item}>
                                    <Input
                                        disabled={isReadOnly}
                                        style={styles.input}
                                        onChangeText={text => this.setState({ number: text })}>
                                        {number}
                                    </Input>
                                </Item>
                            </View>
                        </View>

                        {/* Store Street */}

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreStreet}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <Item regular style={styles.item}>
                                    <Input
                                        disabled={isReadOnly}
                                        style={styles.input}
                                        onChangeText={text => this.setState({ street: text })}>
                                        {street}
                                    </Input>
                                </Item>
                            </View>
                        </View>

                        {/* Store Province */}

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreCity}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <RNPickerSelect
                                    disabled={isReadOnly}
                                    placeholder={{
                                        label: 'Chọn...',
                                        value: null,
                                    }}
                                    items={provinceList}
                                    onValueChange={(value) => {
                                        this.setState({
                                            province: value,
                                        });
                                        this._changeOptionProvince(value);
                                    }}
                                    hideDoneBar={true}
                                    style={{ ...pickerSelectStyles }}
                                    value={province}
                                    ref={(el) => {
                                        this.inputRefs.picker = el;
                                    }}
                                />
                            </View>
                        </View>

                        {/* Store District */}

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreDistrict}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <RNPickerSelect
                                    disabled={isReadOnly}
                                    placeholder={{
                                        label: 'Chọn...',
                                        value: null,
                                    }}
                                    items={districtList}
                                    onValueChange={(value) => {
                                        this.setState({
                                            district: value,
                                        });
                                        this._changeOptionDistrict(value);
                                    }}
                                    hideDoneBar={true}
                                    style={{ ...pickerSelectStyles }}
                                    value={district}
                                    ref={(el) => {
                                        this.inputRefs.picker2 = el;
                                    }}
                                />
                            </View>
                        </View>

                        {/* Store Ward */}

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreWard}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <RNPickerSelect
                                    disabled={isReadOnly}
                                    placeholder={{
                                        label: 'Chọn...',
                                        value: null,
                                    }}
                                    items={wardList}
                                    onValueChange={(value) => {
                                        this.setState({
                                            ward: value,
                                        });
                                    }}
                                    hideDoneBar={true}
                                    style={{ ...pickerSelectStyles }}
                                    value={ward}
                                    ref={(el) => {
                                        this.inputRefs.picker3 = el;
                                    }}
                                />
                            </View>
                        </View>

                        {/* Store type */}

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{'Loại hình cửa hàng'}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <RNPickerSelect
                                    disabled={isReadOnly}
                                    placeholder={{
                                        label: 'Chọn...',
                                        value: null,
                                    }}
                                    items={storeTypeList}
                                    onValueChange={(value) => {
                                        this.setState({
                                            storeType: value,
                                        });
                                    }}
                                    hideDoneBar={true}
                                    style={{ ...pickerSelectStyles }}
                                    value={storeType}
                                    ref={(el) => {
                                        this.inputRefs.picker = el;
                                    }}
                                />
                            </View>
                        </View>

                        {/* Update */}

                        <View style={styles.forgetContainer}>
                            <Text
                                onPress={this.updateStore}
                                style={styles.forgetText}>
                                {'Sửa thông tin cửa hàng'}
                            </Text>
                        </View>

                        <View style={styles.line} />

                        <Text style={styles.title}>{'Vĩ độ: '} {initialPosition ? initialPosition.coords.latitude : ''}</Text>
                        <Text style={styles.title}>{'Kinh độ: '} {initialPosition ? initialPosition.coords.longitude : ''}</Text>

                        <View style={styles.line} />

                        {/* Note */}

                        <Text style={styles.title}>Ghi chú</Text>

                        <Form style={{ alignSelf: 'stretch' }}>
                            <Textarea rowSpan={4} bordered style={styles.input}
                                onChangeText={text => this.setState({ Address: text })}>
                                {note}
                            </Textarea>
                        </Form>

                        <View style={styles.line} />

                        <RadioGroup flexDirection='row' radioButtons={this.state.data} onPress={value => this.handleOptionChange(value)} />

                        {
                            isOK ? this.renderOK() : this.renderNotOK()
                        }

                        {/* Save to local */}

                        <View style={{ marginTop: 20 }}>
                            <MainButton
                                style={styles.button}
                                title={'Lưu'}
                                onPress={this.takeSelfiePhoto} />
                        </View>

                    </ScrollView>
                </View>
            </View>
        );
    }

    renderCamera() {

        const { cameraType } = this.state;

        return (
            <View style={styles.container}>
                <RNCamera
                    ref={ref => {
                        this.camera = ref;
                    }}
                    style={styles.preview}
                    autoFocusPointOfInterest={{ x: 0.5, y: 0.5 }}
                    type={cameraType}
                    flashMode={RNCamera.Constants.FlashMode.auto}
                    permissionDialogTitle={'Permission to use camera'}
                    permissionDialogMessage={'We need your permission to use your camera phone'}
                    onGoogleVisionBarcodesDetected={({ barcodes }) => {
                        console.log(barcodes)
                    }}
                />
                <View style={{ flex: 0, flexDirection: 'row', justifyContent: 'center', margin: 5 }}>
                    <TouchableOpacity
                        onPress={this.takePicture.bind(this)}
                        style={styles.capture}>
                        <Text style={{ fontSize: 14 }}> CHỤP </Text>
                    </TouchableOpacity>
                    <TouchableOpacity
                        onPress={this.changeCameraType}
                        style={styles.capture}>
                        <Text style={{ fontSize: 14 }}> {cameraType === 'back' ? 'CAM TRƯỚC' : 'CAM SAU'} </Text>
                    </TouchableOpacity>
                </View>
            </View>
        );
    };

    renderPreview() {

        const { urlNow } = this.state;

        return (
            <View style={styles.container}>
                <Image
                    style={styles.preview}
                    source={{ uri: urlNow }}
                    resizeMode="cover"
                />
                <View style={{ flex: 0, flexDirection: 'row', justifyContent: 'center', margin: 5 }}>
                    <TouchableOpacity
                        onPress={this.handleTakePhoto.bind(this)}
                        style={styles.capture}>
                        <Text style={{ fontSize: 14 }}> CHỤP LẠI </Text>
                    </TouchableOpacity>
                    <TouchableOpacity
                        onPress={this.okPicture.bind(this)}
                        style={styles.capture}>
                        <Text style={{ fontSize: 14 }}> OK </Text>
                    </TouchableOpacity>
                </View>
            </View>
        );
    }

    render() {

        const { isCamera, isMain, isPreview } = this.state;

        return (
            <View style={styles.container}>
                {
                    isCamera ? this.renderCamera() : <View />
                }
                {
                    isPreview ? this.renderPreview() : <View />
                }
                {
                    isMain ? this.renderMain() : <View />
                }
            </View>
        );
    }
}

const styles = StyleSheet.create({
    container: {
        flex: 1
    },
    subContainer: {
        flex: 1,
        flexDirection: 'column',
        padding: 10,
        paddingTop: 0,
        paddingBottom: 0,
        marginBottom: 20
    },
    centerContainer: {
        alignItems: 'center',
        justifyContent: 'center',
        flex: 1,
        flexDirection: 'column',
    },
    rowContainer: {
        alignItems: 'center',
        justifyContent: 'center',
        flex: 1,
        flexDirection: 'row',
    },
    rowContainer2: {
        flex: 1,
        flexDirection: 'row',
        paddingBottom: 10
    },
    imgContainer: {
        flexDirection: 'column',
        justifyContent: 'center'
    },
    title: {
    },
    bottomContainer: {
        flexDirection: 'column',
        justifyContent: 'center',
        alignContent: 'center',
        paddingBottom: 50
    },
    button: {
        height: 40,
        marginBottom: 10,
    },
    button2: {
        height: CARD_HEIGHT_2,
        width: CARD_WIDTH_2,
        marginTop: 0
    },
    line: {
        height: 0.5,
        backgroundColor: COLORS.BLUE_2E5665,
        marginTop: 15,
        marginBottom: 15
    },
    itemSubTitle: {
        fontFamily: FONTS.MAIN_FONT_BOLD,
        marginBottom: 15
    },
    card: {
        padding: 5,
        elevation: 2,
        backgroundColor: "#FFF",
        marginHorizontal: 10,
        shadowColor: "#000",
        shadowRadius: 5,
        shadowOpacity: 0.3,
        shadowOffset: { x: 2, y: -2 },
        height: CARD_HEIGHT,
        width: CARD_WIDTH,
        overflow: "hidden",
        marginBottom: 10
    },
    card2: {
        padding: 5,
        elevation: 2,
        backgroundColor: "#FFF",
        marginHorizontal: 10,
        shadowColor: "#000",
        shadowRadius: 5,
        shadowOpacity: 0.3,
        shadowOffset: { x: 2, y: -2 },
        height: CARD_HEIGHT_2,
        width: CARD_WIDTH_2,
        overflow: "hidden",
        marginBottom: 10
    },
    cardImage: {
        flex: 2,
        width: "100%",
        height: "100%",
        alignSelf: "center",
    },
    leftItem: {
        flex: 0.4,
        justifyContent: 'center',
    },
    rightItem: {
        flex: 0.6,
        justifyContent: 'center',
        alignContent: 'center',
    },
    item: {
        height: 40,
        marginTop: 10
    },
    input: {
        fontFamily: FONTS.MAIN_FONT_REGULAR,
    },
    forgetContainer: {
        alignItems: 'flex-end',
        alignSelf: 'stretch',
        marginTop: 20
    },
    forgetText: {
        color: COLORS.BLUE_2E5665,
        fontFamily: FONTS.MAIN_FONT_BOLD,
    },
    preview: {
        flex: 1,
        justifyContent: 'flex-end',
        alignItems: 'center'
    },
    capture: {
        flex: 0,
        backgroundColor: '#fff',
        borderRadius: 5,
        padding: 15,
        paddingHorizontal: 20,
        alignSelf: 'center',
        margin: 20
    }
});

const pickerSelectStyles = StyleSheet.create({
    inputIOS: {
        fontSize: 16,
        paddingTop: 13,
        paddingHorizontal: 10,
        paddingBottom: 12,
        borderWidth: 1,
        borderColor: 'gray',
        borderRadius: 4,
        color: 'black',
        marginTop: 10
    },
});

function mapStateToProps(state) {
    return {
        isLoading: state.POSMDetailReducer.isLoading,
        dataResListStoreType: state.POSMDetailReducer.dataResListStoreType,
        dataResListProvinces: state.POSMDetailReducer.dataResListProvinces,
        dataResListDistricts: state.POSMDetailReducer.dataResListDistricts,
        dataResListWards: state.POSMDetailReducer.dataResListWards,
        dataResStore: state.POSMDetailReducer.dataResStore,
        error: state.POSMDetailReducer.error,
        errorMessage: state.POSMDetailReducer.errorMessage,

        dataResUser: state.loginReducer.dataRes,

        PushInfoData: state.pushInfoToServerReducer.dataRes,
        PushInfoIsLoading: state.pushInfoToServerReducer.isLoading,
        PushInfoError: state.pushInfoToServerReducer.error,
        PushInfoErrorMessage: state.pushInfoToServerReducer.errorMessage,
    }
}

function dispatchToProps(dispatch) {
    return bindActionCreators({
        fetchDataGetAllStoreType,
        fetchDataGetAllDistrics,
        fetchDataGetAllProvinces,
        fetchDataGetAllWards,
        fetchDataGetStoreByCode,
        fetchPushInfoToServer
    }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(POSMDetail);
