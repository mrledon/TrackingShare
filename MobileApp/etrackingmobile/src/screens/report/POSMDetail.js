import React, { Component } from 'react';
import {
    View, StyleSheet, Image, TouchableOpacity,
    Dimensions, ScrollView, Alert, AsyncStorage,
    CameraRoll
} from 'react-native';
import { Text, Input, Item, Form, Textarea } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import ImagePicker from 'react-native-image-picker';
import RNPickerSelect from 'react-native-picker-select';
import Spinner from 'react-native-loading-spinner-overlay';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import {
    fetchDataGetAllStoreType,
    fetchDataGetAllDistrics,
    fetchDataGetAllProvinces,
    fetchDataGetAllWards,
    fetchDataGetStoreByCode
} from '../../redux/actions/ActionPOSMDetail';

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
            initialPosition: null,

            isReadOnly: true,
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

            STICKER_7UP: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'STICKER_7UP'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'STICKER_7UP'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'STICKER_7UP'
                }
            ],

            STICKER_PEPSI: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'STICKER_PEPSI'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'STICKER_PEPSI'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'STICKER_PEPSI'
                }
            ],

            BANNER_PEPSI: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'BANNER_PEPSI'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'BANNER_PEPSI'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'BANNER_PEPSI'
                }
            ],

            BANNER_7UP_TET: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'BANNER_7UP_TET'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'BANNER_7UP_TET'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'BANNER_7UP_TET'
                }
            ],

            BANNER_MIRINDA: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'BANNER_MIRINDA'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'BANNER_MIRINDA'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'BANNER_MIRINDA'
                }
            ],

            BANNER_TWISTER: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'BANNER_TWISTER'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'BANNER_TWISTER'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'BANNER_TWISTER'
                }
            ],

            BANNER_REVIVE: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'BANNER_REVIVE'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'BANNER_REVIVE'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'BANNER_REVIVE'
                }
            ],

            BANNER_OOLONG: [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: 'BANNER_OOLONG'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: 'BANNER_OOLONG'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: 'BANNER_OOLONG'
                }
            ],

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
            { enableHighAccuracy: true, timeout: 20000, maximumAge: 1000 }
        );
    }

    _getDataSetup = async () => {
        // Call API
        // await this.props.fetchDataGetAllStoreType()
        //     .then(() => setTimeout(() => {
        //         this.bindDataStoreType()
        //     }, 100));

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
        this.props.navigation.navigate('Home');
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

    updateStore = () => {
        this.setState({ isReadOnly: false });

        const { provinceList } = this.state;

        if (provinceList == null) {
            this._getAllProvince();
        }
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
            type: 'TRANH_PEPSI_AND_7UP'
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
            type: 'STICKER_7UP'
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
            type: 'STICKER_PEPSI'
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
            type: 'BANNER_PEPSI'
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
            type: 'BANNER_7UP_TET'
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
            type: 'BANNER_MIRINDA'
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
            type: 'BANNER_TWISTER'
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
            type: 'BANNER_REVIVE'
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
            type: 'BANNER_OOLONG'
        };
        items.push(item);

        this.setState({ BANNER_OOLONG: items });

        this.forceUpdate();

        this.handleTakePhoto('BANNER_OOLONG', item.id);
    }

    takeSelfiePhoto = () => {

        const { storeData } = this.state;

        if (storeData === null) {
            Alert.alert('Lỗi', 'Vui lòng chọn cửa hàng');
            return;
        }

        this.handleTakePhotoSelfie('SELFIE');
    }

    // save data to local

    _storeDataToLocal = async () => {

        const { dataResUser } = this.props;

        const { name, street, number, province,
            district, ward, note, storeData, initialPosition } = this.state;

        const {
            storeIMGAddress, storeIMGOverview, selfie,
            STICKER_7UP, STICKER_PEPSI, BANNER_PEPSI,
            BANNER_7UP_TET, BANNER_MIRINDA, BANNER_OOLONG,
            BANNER_REVIVE, BANNER_TWISTER, TRANH_PEPSI_AND_7UP } = this.state;

        try {
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
                MasterStoreId: storeData.Id,
                Date: moment().format('DD/MM/YYYY'),
                POSM: []
            };

            let listIMGFinal = [];
            let listIMG = [];
            listIMG.concat(STICKER_7UP);
            listIMG.concat(STICKER_PEPSI);
            listIMG.concat(BANNER_PEPSI);
            listIMG.concat(BANNER_7UP_TET);
            listIMG.concat(BANNER_MIRINDA);
            listIMG.concat(BANNER_OOLONG);
            listIMG.concat(BANNER_REVIVE);
            listIMG.concat(BANNER_TWISTER);
            listIMG.concat(TRANH_PEPSI_AND_7UP);

            console.log('imglist', listIMG);

            // Add to list final
            if (selfie !== '') {
                listIMGFinal.push(
                    {
                        Id: dataResUser.Data.Id,
                        Code: 'SELFIE',
                        Date: moment().format('DD/MM/YYYY'),
                        MasterStoreId: storeData.Id,
                        Token: dataResUser.Data.Token,
                        Photo: {
                            uri: selfie,
                            type: 'image/jpeg',
                            name: 'SELFIE'
                        },
                    }
                );
            }

            if (storeIMGAddress !== '') {
                listIMGFinal.push(
                    {
                        Id: dataResUser.Data.Id,
                        Code: 'DEFAULT',
                        Date: moment().format('DD/MM/YYYY'),
                        MasterStoreId: storeData.Id,
                        Token: dataResUser.Data.Token,
                        Photo: {
                            uri: storeIMGAddress,
                            type: 'image/jpeg',
                            name: 'DEFAULT'
                        },
                    }
                );
            }

            if (storeIMGOverview !== '') {
                listIMGFinal.push(
                    {
                        Id: dataResUser.Data.Id,
                        Code: 'DEFAULT',
                        Date: moment().format('DD/MM/YYYY'),
                        MasterStoreId: storeData.Id,
                        Token: dataResUser.Data.Token,
                        Photo: {
                            uri: storeIMGOverview,
                            type: 'image/jpeg',
                            name: 'DEFAULT'
                        },
                    }
                );
            }

            listIMG.forEach(element => {
                if (element.url !== '' && element.url !== null) {
                    listIMGFinal.push(
                        {
                            Id: dataResUser.Data.Id,
                            Code: element.type,
                            Date: moment().format('DD/MM/YYYY'),
                            MasterStoreId: storeData.Id,
                            Token: dataResUser.Data.Token,
                            Photo: {
                                uri: element.url,
                                type: 'image/jpeg',
                                name: element.type,
                            },
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

        } catch (error) {
            Alert.alert('Lỗi', error + '');
        }
    }

    // remove file

    removeFile = (filepath) => {
        RNFS.exists(filepath)
            .then((result) => {
                console.log("file exists: ", result);

                if (result) {
                    return RNFS.unlink(filepath)
                        .then(() => {
                            // Alert.alert('FILE DELETED');
                        })
                        // `unlink` will throw an error, if the item to unlink does not exist
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

    handleTakePhoto(type, id) {

        const options = {
            title: 'Chọn',
            // chooseFromLibraryButtonTitle: 'Thư viện ảnh',
            takePhotoButtonTitle: 'Chụp ảnh',
            cancelButtonTitle: 'Đóng',
            storageOptions: {
                skipBackup: true,
                path: 'images',
            },
        };

        ImagePicker.showImagePicker(options, (response) => {
            console.log('Response = ', response);

            if (response.didCancel) {
                console.log('User cancelled image picker');
            } else if (response.error) {
                console.log('ImagePicker Error: ', response.error);
            } else if (response.customButton) {
                console.log('User tapped custom button: ', response.customButton);
            } else {

                if (type == 'SELFIE') {

                    CameraRoll.saveToCameraRoll(response.uri, 'photo').then((data) => {
                        console.log('photone', data)
                        this.setState({
                            selfie: data,
                        });

                        this._storeDataToLocal();
                    });

                    this.removeFile(response.uri);
                }
                else if (type == 'DEFAULT_1') {

                    CameraRoll.saveToCameraRoll(response.uri, 'photo').then((data) => {
                        console.log('photone', data)
                        this.setState({
                            storeIMGOverview: data,
                        });
                    });

                    this.removeFile(response.uri);
                }
                else if (type == 'DEFAULT_2') {

                    CameraRoll.saveToCameraRoll(response.uri, 'photo').then((data) => {
                        console.log('photone', data)
                        this.setState({
                            storeIMGAddress: data,
                        });
                    });

                    this.removeFile(response.uri);
                }
                else if (type == 'STICKER_7UP') {

                    CameraRoll.saveToCameraRoll(response.uri, 'photo').then((data) => {

                        const items = this.state.STICKER_7UP;
                        items[id].url = data;

                        this.forceUpdate();
                    });

                    this.removeFile(response.uri);
                } else if (type == 'STICKER_PEPSI') {

                    CameraRoll.saveToCameraRoll(response.uri, 'photo').then((data) => {

                        const items = this.state.STICKER_PEPSI;
                        items[id].url = data;

                        this.forceUpdate();
                    });

                    this.removeFile(response.uri);
                } else if (type == 'BANNER_PEPSI') {

                    CameraRoll.saveToCameraRoll(response.uri, 'photo').then((data) => {

                        const items = this.state.BANNER_PEPSI;
                        items[id].url = data;

                        this.forceUpdate();
                    });

                    this.removeFile(response.uri);
                }
                else if (type == 'BANNER_7UP_TET') {

                    CameraRoll.saveToCameraRoll(response.uri, 'photo').then((data) => {

                        const items = this.state.BANNER_7UP_TET;
                        items[id].url = data;

                        this.forceUpdate();
                    });

                    this.removeFile(response.uri);
                }
                else if (type == 'BANNER_MIRINDA') {

                    CameraRoll.saveToCameraRoll(response.uri, 'photo').then((data) => {

                        const items = this.state.BANNER_MIRINDA;
                        items[id].url = data;

                        this.forceUpdate();
                    });

                    this.removeFile(response.uri);
                }
                else if (type == 'BANNER_TWISTER') {

                    CameraRoll.saveToCameraRoll(response.uri, 'photo').then((data) => {

                        const items = this.state.BANNER_TWISTER;
                        items[id].url = data;

                        this.forceUpdate();
                    });

                    this.removeFile(response.uri);
                }
                else if (type == 'BANNER_REVIVE') {

                    CameraRoll.saveToCameraRoll(response.uri, 'photo').then((data) => {

                        const items = this.state.BANNER_REVIVE;
                        items[id].url = data;

                        this.forceUpdate();
                    });

                    this.removeFile(response.uri);
                }
                else if (type == 'BANNER_OOLONG') {

                    CameraRoll.saveToCameraRoll(response.uri, 'photo').then((data) => {

                        const items = this.state.BANNER_OOLONG;
                        items[id].url = data;

                        this.forceUpdate();
                    });

                    this.removeFile(response.uri);
                }
                else if (type == 'TRANH_PEPSI_AND_7UP') {

                    CameraRoll.saveToCameraRoll(response.uri, 'photo').then((data) => {

                        const items = this.state.TRANH_PEPSI_AND_7UP;
                        items[id].url = data;

                        this.forceUpdate();
                    });

                    this.removeFile(response.uri);
                }
            }
        });
    }

    handleTakePhotoSelfie(type) {

        const options = {
            title: 'Chọn',
            // chooseFromLibraryButtonTitle: 'Thư viện ảnh',
            takePhotoButtonTitle: 'Chụp ảnh chân dung',
            cancelButtonTitle: 'Đóng',
            cameraType: 'front',
            storageOptions: {
                skipBackup: true,
                path: 'images',
            },
        };

        ImagePicker.showImagePicker(options, (response) => {
            console.log('Response = ', response);

            if (response.didCancel) {
                console.log('User cancelled image picker');
            } else if (response.error) {
                console.log('ImagePicker Error: ', response.error);
            } else if (response.customButton) {
                console.log('User tapped custom button: ', response.customButton);
            } else {

                if (type == 'SELFIE') {
                    this.setState({
                        selfie: response.uri,
                    });

                    this._storeDataToLocal();
                }
            }
        });
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

    render() {

        const { isLoading } = this.props;

        const { storeTypeList, storeType,
            provinceList, province,
            districtList, district,
            wardList, ward, note,
            code, name, street, number, isReadOnly,
            storeIMGAddress, storeIMGOverview,
            STICKER_7UP, STICKER_PEPSI, BANNER_PEPSI,
            BANNER_7UP_TET, BANNER_MIRINDA, BANNER_OOLONG,
            BANNER_REVIVE, BANNER_TWISTER, TRANH_PEPSI_AND_7UP,
            initialPosition } = this.state;

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
                            marginTop: 10
                        }}
                        style={{ padding: 0 }}>

                        {/* <View style={styles.rowContainer}>
                            <View style={{ width: width - 100, marginBottom: 10 }}>
                                <RNPickerSelect
                                    placeholder={{
                                        label: 'Loại hình cửa hàng',
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
                        </View> */}

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

                        {/* Update */}

                        <View style={styles.forgetContainer}>
                            <Text
                                onPress={this.updateStore}
                                style={styles.forgetText}>
                                {'Cập nhật'}
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

                        {/* Sticker 7Up */}

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

                        {/* Save to local */}

                        <MainButton
                            style={styles.button}
                            title={'Lưu'}
                            onPress={this.takeSelfiePhoto} />

                    </ScrollView>
                </View>
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
    }
}

function dispatchToProps(dispatch) {
    return bindActionCreators({
        fetchDataGetAllStoreType,
        fetchDataGetAllDistrics,
        fetchDataGetAllProvinces,
        fetchDataGetAllWards,
        fetchDataGetStoreByCode
    }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(POSMDetail);
