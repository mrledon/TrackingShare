import React, { Component } from 'react';
import { View, StyleSheet, Image, TouchableOpacity, Dimensions, ScrollView, Alert, Picker, AsyncStorage } from 'react-native';
import { Text, Input, Item, Form, Textarea } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import ImagePicker from 'react-native-image-picker';
import RNPickerSelect from 'react-native-picker-select';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import {
    fetchDataGetAllStoreType,
    fetchDataGetAllDistrics,
    fetchDataGetAllProvinces,
    fetchDataGetAllWards,
    fetchDataGetStoreByCode
} from '../../redux/actions/ActionPOSMDetail';

import {
    fetchPushDataToServer
} from '../../redux/actions/ActionPushDataToServer';

const LOGO = require('../../assets/images/default.jpg');

const { width, height } = Dimensions.get("window");

const CARD_WIDTH = width / 2 - 40;
const CARD_HEIGHT = CARD_WIDTH - 20;

const CARD_WIDTH_2 = width / 3 - 20;
const CARD_HEIGHT_2 = CARD_WIDTH_2 - 10;

class POSMDetail extends Component {
    constructor(props) {
        super(props);
        this.inputRefs = {};
        this.state = {
            base64Icon: 'dffsd',

            isReadOnly: true,

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

        };
    }

    componentWillMount() {
        this._getAllStoreType();
    }

    _getAllStoreType = async () => {
        // Call API
        // await this.props.fetchDataGetAllStoreType()
        //     .then(() => setTimeout(() => {
        //         this.bindDataStoreType()
        //     }, 100));

        await this.props.fetchDataGetAllProvinces()
            .then(() => setTimeout(() => {
                this.bindDataProvince()
            }, 100));

        // await this.props.fetchDataGetAllDistrics('2')
        //     .then(() => setTimeout(() => {
        //         this.bindDataDistrict()
        //     }, 100));

        // await this.props.fetchDataGetAllWards()
        //     .then(() => setTimeout(() => {
        //         this.bindDataWard()
        //     }, 100));
    }

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

    handleBack = () => {
        this.props.navigation.navigate('Home');
    }

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
                    name: dataResStore.Data.Name, number: dataResStore.Data.HouseNumber,
                    street: dataResStore.Data.StreetNames, province: dataResStore.Data.ProvinceId,
                    district: dataResStore.Data.DistrictId, ward: dataResStore.Data.WardId
                });

            }
        }
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

    _storeDataToLocal = async () => {

        try {

            let posm = {
                Id: '305478924',
                Code: 'DEFAULT',
                Date: '12/11/2018',
                MasterStoreId: '65863be6-896b-48dd-8b8a-9e065b149461',
                Token: 'ebea44c6-1704-4eb6-a4c7-432ddad846e6',
                Photo: {
                    uri: this.state.base64Icon,
                    type: 'image/jpeg',
                    name: 'testPhotoName'
                },
            };


            await AsyncStorage.setItem('DATA_SSC', JSON.stringify(posm));

            Alert.alert('ok');

        } catch (error) {
            // Error saving data
            Alert.alert(error + '');
        }
    }

    _pushDataToServer = async () => {

        this._storeDataToLocal();

        // await this.props.fetchPushDataToServer('11', '22', '11', '22', '11', this.state.base64Icon)
        //     .then(() => setTimeout(() => {
        //         this.hello();
        //     }, 100));
    }

    hello = () => {
        const { PUSHdataRes, PUSHerror, PUSHerrorMessage } = this.props;

        if (PUSHerror) {
            let _mess = PUSHerrorMessage + '';
            if (PUSHerrorMessage == 'TypeError: Network request failed')
                _mess = STRINGS.MessageNetworkError;

            Alert.alert(
                STRINGS.MessageTitleError, _mess,
                [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
            );
            return;
        }
        else {
            if (PUSHdataRes.HasError == true) {
                Alert.alert(
                    STRINGS.MessageTitleError, PUSHdataRes.Message + '',
                    [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
                );
            } else if (PUSHdataRes.HasError == false) {
                Alert.alert('Upload thanh cong');
            }
        }
    }


    handleTakePhoto(type, id) {

        const options = {
            title: 'Chọn',
            chooseFromLibraryButtonTitle: 'Thư viện ảnh',
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
                // Alert.alert(response.uri);
                //   const source = { uri: response.uri };

                //   // You can also display the image using data:
                //   // const source = { uri: 'data:image/jpeg;base64,' + response.data };

                if (type == 'DEFAULT_1') {
                    this.setState({
                        storeIMGOverview: response.uri,
                    });
                }
                else if (type == 'DEFAULT_2') {
                    this.setState({
                        storeIMGAddress: response.uri,
                    });
                }

                if (type == 'STICKER_7UP') {
                    const items = this.state.STICKER_7UP;
                    items[id].url = response.uri;

                    // re-render
                    this.forceUpdate();
                }
            }
        });
    }

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

        const { storeTypeList, storeType,
            provinceList, province,
            districtList, district,
            wardList, ward,
            code, name, street, number, isReadOnly,
            storeIMGAddress, storeIMGOverview,
            STICKER_7UP, } = this.state;

        return (
            <View
                style={styles.container}>
                <MainHeader
                    onPress={() => this.handleBack()}
                    hasLeft={true}
                    title={STRINGS.POSMDetailTitle} />
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

                        <Text style={styles.title}>{'Kinh độ: 12.123456  Vĩ độ: 45.67893'}</Text>

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

                        <Text style={styles.title}>Ghi chú</Text>

                        <Form style={{ alignSelf: 'stretch' }}>
                            <Textarea rowSpan={4} bordered style={styles.input}
                                onChangeText={text => this.setState({ Address: text })}>
                            </Textarea>
                        </Form>

                        <MainButton
                            style={styles.button}
                            title={'Lưu'}
                            onPress={this._pushDataToServer} />

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

        PUSHdataRes: state.pushDataToServerReducer.dataRes,
        PUSHerror: state.pushDataToServerReducer.error,
        PUSHerrorMessage: state.pushDataToServerReducer.errorMessage,
    }
}

function dispatchToProps(dispatch) {
    return bindActionCreators({
        fetchDataGetAllStoreType,
        fetchDataGetAllDistrics,
        fetchDataGetAllProvinces,
        fetchDataGetAllWards,
        fetchDataGetStoreByCode,
        fetchPushDataToServer
    }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(POSMDetail);
