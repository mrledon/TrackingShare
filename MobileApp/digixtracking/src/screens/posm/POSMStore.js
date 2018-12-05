import React, { Component } from 'react';
import {
    View, StyleSheet, ScrollView, Alert,
    AsyncStorage, BackHandler
} from 'react-native';
import { Text, Input, Item, Form, Textarea } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import RNPickerSelect from 'react-native-picker-select';
import Spinner from 'react-native-loading-spinner-overlay';
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

import { savePOSM } from '../../redux/actions/ActionPOSM';
import { fetchPushInfoToServer } from '../../redux/actions/ActionPushInfoToServer';
var moment = require('moment');

class POSMStore extends Component {

    constructor(props) {
        super(props);
        this.inputRefs = {};
        this.state = {
            isStoreChange: false,
            initialPosition: null,
            isReadOnly: true,
            isOK: true,
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

            note: '',
            code: '',
            name: '',
            number: '',
            street: '',
            phone: '',

            storeTypeList: [],
            storeType: undefined,

            provinceList: [],
            province: undefined,

            districtList: [],
            district: undefined,

            wardList: [],
            ward: undefined,

            storeData: null
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
            (error) => console.log(error.message),
            { enableHighAccuracy: false, timeout: 20000, maximumAge: 3000 }
        );

        BackHandler.addEventListener('hardwareBackPress', this.handleBackPress);
    }

    handleBackPress = () => {
        Alert.alert(
            STRINGS.MessageTitleAlert, 'Bạn chưa hoàn thành tiến trình công việc, bạn có chắc chắn thoát trang này, dữ liệu sẽ bị mất ?',
            [{
                text: STRINGS.MessageActionOK, onPress: () => {
                    this.props.navigation.navigate('Home');
                    return false;
                }
            },
            { text: STRINGS.MessageActionCancel, onPress: () => console.log('Cancel Pressed') }],
            { cancelable: false }
        );

        return true;
    }

    _getDataSetup = async () => {
        // Call API
        await this.props.fetchDataGetAllProvinces()
            .then(() => setTimeout(() => {
                this.bindDataProvince()
            }, 100));

        setTimeout(() => {
            this.props.fetchDataGetAllStoreType()
                .then(() => setTimeout(() => {
                    this.bindDataStoreType()
                }, 100));
        }, 1000)

        // await AsyncStorage.removeItem('POSM_SSC');
        this.props.savePOSM(null);
    }

    // change combobox

    _changeOptionProvince = (id) => {

        setTimeout(() => {
            this.props.fetchDataGetAllDistrics(id)
                .then(() => setTimeout(() => {
                    this.bindDataDistrict()
                }, 100));
        }, 1000);

    }

    _changeOptionDistrict = (id) => {

        setTimeout(() => {
            this.props.fetchDataGetAllWards(id)
                .then(() => setTimeout(() => {
                    this.bindDataWard()
                }, 100));
        }, 1000);

    }

    // find store
    handleFindStore = () => {

        this.setState({ isReadOnly: true, districtList: [], district: undefined, wardList: [], ward: undefined });

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
        this.setState({ isReadOnly: false, isStoreChange: true });
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
                    district: dataResStore.Data.DistrictId, ward: dataResStore.Data.WardId,
                    phone: dataResStore.Data.PhoneNumber,
                    storeType: dataResStore.Data.StoreType
                });

            }
        }
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

    // save data
    _storeDataToLocal = async (trackId) => {

        const { name, code, isOK } = this.state;

        try {
            let item = {
                Name: name,
                Code: code,
                TrackId: trackId,
                Date: moment().format('DD/MM/YYYY'),
                POSM: []
            };

            this.props.savePOSM(item);

            // await AsyncStorage.setItem('POSM_SSC', JSON.stringify(item));

            setTimeout(() => {
                if (isOK)
                    this.props.navigation.navigate("POSMOption");
                else
                    this.props.navigation.navigate("POSMFail");
            }, 500);

        } catch (error) {
            Alert.alert('Lỗi');
        }
    }

    // push info to server
    _pushInfoToServer = async () => {

        const { storeData, isReadOnly, name, phone, street, number, province, district, ward, storeType } = this.state;

        if (storeData === null && isReadOnly) {
            Alert.alert('Lỗi', 'Vui lòng chọn cửa hàng');
            return false;
        }
        else if (!isReadOnly && (name == '' || phone == '' || street == '' || number == '' ||
            province == undefined || district == undefined || ward == undefined || storeType == undefined)) {
            Alert.alert('Lỗi', 'Vui lòng nhập đầy đủ thông tin cửa hàng');
            return false;
        }

        const { dataResUser } = this.props;

        const { note, initialPosition, isOK, isStoreChange } = this.state;

        var storeID = '';
        if (storeData !== null) {
            storeID = storeData.Id;
        }

        let item = {
            Token: dataResUser.Data.Token,
            Id: dataResUser.Data.Id,
            MaterStoreName: name,
            HouseNumber: number,
            PhoneNumber: phone,
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
            StoreStatus: isOK ? true : false,
            StoreIsChanged: isStoreChange,
            StoreType: storeType
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

    renderMain() {

        const { isLoading, PushInfoIsLoading } = this.props;

        const { storeTypeList, storeType,
            provinceList, province,
            districtList, district,
            wardList, ward, note,
            code, name, street, number, isReadOnly,
            initialPosition, phone } = this.state;

        return (
            <View
                style={styles.container}>
                <MainHeader
                    onPress={() => this.handleBackPress()}
                    hasLeft={true}
                    title={STRINGS.POSMDetailTitle} />
                <Spinner visible={isLoading} />
                <Spinner visible={PushInfoIsLoading} />
                <View
                    padder
                    style={styles.subContainer}>

                    <ScrollView
                        ref={(scroller) => { this._scrollView = scroller }}
                        pagingEnabled={false}
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

                        {/* PhoneNumber */}
                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{'Số điện thoại'}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <Item regular style={styles.item}>
                                    <Input
                                        disabled={isReadOnly}
                                        style={styles.input}
                                        onChangeText={text => this.setState({ phone: text })}>
                                        {phone}
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
                                        if (value != undefined) {
                                            this._changeOptionProvince(value);
                                        }
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
                                        if (value != undefined) {
                                            this._changeOptionDistrict(value);
                                        }
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
                                onChangeText={text => this.setState({ note: text })}>
                                {note}
                            </Textarea>
                        </Form>

                        <View style={styles.line} />

                        <RadioGroup flexDirection='row' radioButtons={this.state.data} onPress={value => this.handleOptionChange(value)} />

                        {/* Save to local */}

                        <View style={{ marginTop: 20 }}>
                            <MainButton
                                style={styles.button}
                                title={'Tiếp tục'}
                                onPress={this._pushInfoToServer} />
                        </View>

                    </ScrollView>
                </View>
            </View>
        );
    }

    render() {

        return (
            <View style={styles.container}>
                {
                    this.renderMain()
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
    rowContainer: {
        alignItems: 'center',
        justifyContent: 'center',
        flex: 1,
        flexDirection: 'row',
    },
    button: {
        height: 40,
        marginBottom: 10,
    },
    line: {
        height: 0.5,
        backgroundColor: COLORS.BLUE_2E5665,
        marginTop: 15,
        marginBottom: 15
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
        fetchPushInfoToServer,
        savePOSM
    }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(POSMStore);