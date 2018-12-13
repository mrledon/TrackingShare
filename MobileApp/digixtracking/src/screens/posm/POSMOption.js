import React, { Component } from 'react';
import {
    View, StyleSheet, Alert, AsyncStorage,
    ScrollView, Dimensions, BackHandler,
    Platform, TouchableOpacity, CameraRoll
} from 'react-native';
import { Icon } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';
import { RNCamera } from 'react-native-camera';
import Spinner from 'react-native-loading-spinner-overlay';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import { savePOSM } from '../../redux/actions/ActionPOSM';
import { fetchDataPosmUpdate } from '../../redux/actions/ActionPOSMUpdate';

const { width, height } = Dimensions.get("window");

const CARD_WIDTH = width - 40;
var moment = require('moment');

class POSMOption extends Component {
    constructor(props) {
        super(props);
        var type = props.navigation.getParam('type', '');
        this.state = {
            cameraType: 'front',
            type: type,
            isCamera: false,
            data: [
                {
                    title: 'Ảnh cửa hàng',
                    screen: 'POSMTakePhoto',
                    type: 'DEFAULT',
                    count: 0
                },
                {
                    title: 'Tranh Pepsi & 7Up',
                    screen: 'POSMTakePhoto',
                    type: 'TRANH_PEPSI_AND_7UP',
                    count: 0
                },
                {
                    title: 'Sticker 7Up',
                    screen: 'POSMTakePhoto',
                    type: 'STICKER_7UP',
                    count: 0
                },
                {
                    title: 'Sticker Pepsi',
                    screen: 'POSMTakePhoto',
                    type: 'STICKER_PEPSI',
                    count: 0
                },
                {
                    title: 'Banner Pepsi',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_PEPSI',
                    count: 0
                },
                {
                    title: 'Banner 7Up Tết',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_7UP_TET',
                    count: 0
                },
                {
                    title: 'Banner Mirinda',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_MIRINDA',
                    count: 0
                },
                {
                    title: 'Banner Twister',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_TWISTER',
                    count: 0
                },
                {
                    title: 'Banner Revive',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_REVIVE',
                    count: 0
                },
                {
                    title: 'Banner Olong',
                    screen: 'POSMTakePhoto',
                    type: 'BANNER_OOLONG',
                    count: 0
                },
            ],
            dataFail: [
                {
                    title: 'Ảnh cửa hàng thất bại',
                    screen: 'POSMTakePhoto',
                    type: 'STORE_FAILED',
                    count: 0
                }
            ]
        };
    }

    componentDidMount = () => {
        BackHandler.addEventListener('hardwareBackPress', this.handleBackPress);
    }

    componentWillUnmount = () => {
        BackHandler.removeEventListener('hardwareBackPress', this.handleBackPress);
    }

    handleBackPress = () => {

        const { isCamera } = this.state;
        if (isCamera) {
            this.handleBackCam();
            return true;
        }
        else {
            Alert.alert(
                STRINGS.MessageTitleAlert, 'Bạn chưa hoàn thành tiến trình công việc, bạn có chắc chắn thoát trang này, dữ liệu sẽ bị mất?',
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
    }

    changeNumPOSM = (type) => {
        BackHandler.addEventListener('hardwareBackPress', this.handleBackPress);
        const { dataResPOSM } = this.props;

        if (dataResPOSM != null) {
            var _posmParse = dataResPOSM;
            var _data = this.state.data;

            for (let index = 0; index < _data.length; index++) {
                const element = _data[index];
                if (element.type === type) {
                    _data[index].count = 0;
                }
            }

            if (_posmParse.POSM != null && _posmParse.POSM.length != 0) {
                for (let index = 0; index < _posmParse.POSM.length; index++) {
                    const element = _posmParse.POSM[index];

                    if (element.Code != type)
                        continue;
                    switch (element.Code) {
                        case 'DEFAULT':
                            _data[0].count += 1;
                            break;
                        case 'TRANH_PEPSI_AND_7UP':
                            _data[1].count += 1;
                            break;
                        case 'STICKER_7UP':
                            _data[2].count += 1;
                            break;
                        case 'STICKER_PEPSI':
                            _data[3].count += 1;
                            break;
                        case 'BANNER_PEPSI':
                            _data[4].count += 1;
                            break;
                        case 'BANNER_7UP_TET':
                            _data[5].count += 1;
                            break;
                        case 'BANNER_MIRINDA':
                            _data[6].count += 1;
                            break;
                        case 'BANNER_TWISTER':
                            _data[7].count += 1;
                            break;
                        case 'BANNER_REVIVE':
                            _data[8].count += 1;
                            break;
                        case 'BANNER_OOLONG':
                            _data[9].count += 1;
                            break;
                    }
                }
            }

            this.setState({ data: _data });
        }
    }

    changeNumPOSMFail = (type) => {
        BackHandler.addEventListener('hardwareBackPress', this.handleBackPress);
        const { dataResPOSM } = this.props;

        if (dataResPOSM != null) {
            var _posmParse = dataResPOSM;
            var _data = this.state.dataFail;

            for (let index = 0; index < _data.length; index++) {
                const element = _data[index];
                if (element.type === type) {
                    _data[index].count = 0;
                }
            }

            if (_posmParse.POSM != null && _posmParse.POSM.length != 0) {
                for (let index = 0; index < _posmParse.POSM.length; index++) {
                    const element = _posmParse.POSM[index];

                    if (element.Code != type)
                        continue;
                    _data[0].count += 1;
                }
            }

            this.setState({ dataFail: _data });
        }
    }

    handleGoto = (screen, type) => {

        BackHandler.removeEventListener('hardwareBackPress', this.handleBackPress);

        if (this.state.type) {
            this.props.navigation.navigate(screen, {
                type: type,
                refresh: this.changeNumPOSM.bind(this)
            });
        } else {
            this.props.navigation.navigate(screen, {
                type: type,
                refresh: this.changeNumPOSMFail.bind(this)
            });
        }
    }

    takePicture = async function () {

        if (this.camera) {
            const options = {
                quality: 1, base64: true, width: 1080, height: 1920,
                fixOrientation: true, forceUpOrientation: true, skipProcessing: true
            };
            const data = await this.camera.takePictureAsync(options);

            CameraRoll.saveToCameraRoll(data.uri, 'photo').then((res) => {

                const { dataResUser, dataResPOSM } = this.props;

                var itemAdd = {
                    Id: dataResUser.Data.Id,
                    Code: 'SELFIE',
                    Code2: '',
                    Date: moment().format('DD/MM/YYYY HH:mm:ss'),
                    Token: dataResUser.Data.Token,
                    TrackSessionId: dataResPOSM.TrackId,
                    PosmNumber: 0,
                    Photo: {
                        uri: res,
                        type: 'image/jpeg',
                        name: 'SELFIE',
                    },
                    IsSubmit: false
                }

                dataResPOSM.POSM.push(itemAdd);
                this.props.savePOSM(dataResPOSM);

                setTimeout(() => {
                    this.updateData();
                }, 500);
            });
        }
    };

    handleBackCam = () => {
        this.setState({ isCamera: false });
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

    handleDone = async () => {
        try {
            const { data, type, dataFail } = this.state;
            if (data[0].count < 2 && type) {
                Alert.alert('Lỗi', 'Vui lòng chụp ít nhất 2 ảnh cửa hàng');
                return;
            } else if (dataFail[0].count < 2 && !type) {
                Alert.alert('Lỗi', 'Vui lòng chụp ít nhất 2 ảnh cửa hàng thất bại');
                return;
            }

            if (type) {
                this.setState({ isCamera: true });
                this.forceUpdate();
            }
            else {
                this.updateData();
            }
        } catch (error) {
            Alert.alert('Lỗi');
        }
    }

    updateData = () => {
        const { dataResPOSM } = this.props;

        if (dataResPOSM != null) {
            this.props.fetchDataPosmUpdate(dataResPOSM.TrackId)
                .then(() => setTimeout(() => {
                    this.bindUpdateData()
                }, 100));
        }
    }

    bindUpdateData() {

        const { dataRes, error, errorMessage } = this.props;

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
            if (dataRes.HasError == true) {
                Alert.alert(
                    STRINGS.MessageTitleError, dataRes.Message + '',
                    [{ text: STRINGS.MessageActionOK, onPress: () => console.log('OK Pressed') }], { cancelable: false }
                );
            } else if (dataRes.HasError == false) {
                this.saveData();
            }
        }
    }

    saveData = async () => {
        try {
            const _data = await AsyncStorage.getItem('DATA_SSC');
            const { dataResPOSM } = this.props;

            if (dataResPOSM != null) {
                let _posmParse = dataResPOSM;

                if (_data != null) {
                    let dataParse = JSON.parse(_data);
                    dataParse.push(_posmParse);
                    await AsyncStorage.setItem('DATA_SSC', JSON.stringify(dataParse));
                }
                else {
                    var _newData = [];
                    _newData.push(_posmParse);
                    await AsyncStorage.setItem('DATA_SSC', JSON.stringify(_newData));
                }

                Alert.alert(
                    STRINGS.MessageTitleAlert, 'Lưu dữ liệu thành công',
                    [{
                        text: STRINGS.MessageActionOK, onPress: () => {

                            this.props.navigation.navigate('Home');
                        }
                    }],
                    { cancelable: false }
                );
            }
        } catch (error) {
            Alert.alert('Lỗi');
        }
    }

    render() {

        const { data, dataFail, type, isCamera, cameraType } = this.state;
        const { isLoading } = this.props;

        return (
            <View style={styles.container}>
                <Spinner visible={isLoading} />
                {
                    !isCamera ?
                        <View style={styles.container}>
                            <MainHeader
                                hasLeft={false}
                                title={'Chọn loại ảnh'} />
                            <View
                                padder
                                style={styles.subContainer}>

                                <ScrollView
                                    horizontal={false}
                                    showsHorizontalScrollIndicator={false}
                                    showsVerticalScrollIndicator={false}
                                    style={{ padding: 0, width: CARD_WIDTH, marginBottom: 20 }}>
                                    {
                                        type ?
                                            data.map((item, index) => {
                                                return (
                                                    <MainButton
                                                        style={styles.button}
                                                        styleTitle={item.count != 0 ? styles.buttonTitleRed : styles.buttonTitleWhite}
                                                        title={item.title + ' (' + item.count.toString() + ')'}
                                                        onPress={() => this.handleGoto(item.screen, item.type)} />
                                                );
                                            })
                                            :
                                            dataFail.map((item, index) => {
                                                return (
                                                    <MainButton
                                                        style={styles.button}
                                                        styleTitle={item.count != 0 ? styles.buttonTitleRed : styles.buttonTitleWhite}
                                                        title={item.title + ' (' + item.count.toString() + ')'}
                                                        onPress={() => this.handleGoto(item.screen, item.type)} />
                                                );
                                            })
                                    }
                                </ScrollView>

                                <MainButton
                                    style={styles.button2}
                                    title={'HOÀN THÀNH'}
                                    onPress={this.handleDone} />

                            </View>
                        </View>
                        :
                        <View style={styles.container}>
                            <RNCamera
                                ref={ref => {
                                    this.camera = ref;
                                }}
                                style={styles.preview}
                                autoFocusPointOfInterest={{ x: 0.5, y: 0.5 }}
                                type={cameraType}
                                flashMode={RNCamera.Constants.FlashMode.off}
                                permissionDialogTitle={'Permission to use camera'}
                                permissionDialogMessage={'We need your permission to use your camera phone'}
                                pauseAfterCapture={true}
                                onGoogleVisionBarcodesDetected={({ barcodes }) => {
                                    console.log(barcodes)
                                }}
                            />
                            <View style={{ flex: 0, flexDirection: 'row', justifyContent: 'center', marginTop: 5 }}>
                                <TouchableOpacity
                                    onPress={this.handleBackCam}
                                    style={styles.capture}>
                                    <Icon name={'arrow-left'} size={15} type="FontAwesome"></Icon>
                                </TouchableOpacity>
                                <TouchableOpacity
                                    onPress={this.changeCameraType}
                                    style={styles.capture}>
                                    <Icon name={'refresh'} size={15} type="FontAwesome"></Icon>
                                </TouchableOpacity>
                                <TouchableOpacity
                                    onPress={this.takePicture.bind(this)}
                                    style={styles.capture}>
                                    <Icon name={'camera'} size={15} type="FontAwesome"></Icon>
                                </TouchableOpacity>
                            </View>
                        </View>
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
        alignItems: 'center',
        padding: 20
    },
    bottomContainer: {
        flexDirection: 'column',
        justifyContent: 'center',
        alignContent: 'center',
        paddingBottom: 50,
    },
    button: {
        height: 50
    },
    buttonTitleWhite: {
        color: 'white'
    },
    buttonTitleRed: {
        color: 'red'
    },
    button2: {
        height: 50,
        backgroundColor: 'green'
    },
    logo: {
        marginBottom: 20,
        width: 100,
        height: 100
    },
    textBottom: {
        textAlign: 'center',
        fontFamily: FONTS.MAIN_FONT_REGULAR,
    },
    preview: {
        flex: 1,
        justifyContent: 'center',
        alignItems: 'center',
        alignSelf: 'stretch',
        marginTop: Platform.OS == 'android' ? 0 : 30,
        marginBottom: Platform.OS == 'android' ? 20 : 0
    },
    capture: {
        flex: 0,
        backgroundColor: '#fff',
        borderRadius: 5,
        padding: 10,
        alignSelf: 'center',
        marginHorizontal: 10,
        marginVertical: 5,
        alignItems: 'center',
        width: 55
    },
});

function mapStateToProps(state) {
    return {
        dataResUser: state.loginReducer.dataRes,
        dataResPOSM: state.POSMReducer.dataRes,

        isLoading: state.POSMUpdateReducer.isLoading,
        dataRes: state.POSMUpdateReducer.dataRes,
        error: state.POSMUpdateReducer.error,
        errorMessage: state.POSMUpdateReducer.errorMessage
    }
}

function dispatchToProps(dispatch) {
    return bindActionCreators({
        savePOSM,
        fetchDataPosmUpdate
    }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(POSMOption);
