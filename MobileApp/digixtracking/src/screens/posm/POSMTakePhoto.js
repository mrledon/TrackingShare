import React, { Component } from 'react';
import {
    Dimensions,
    StyleSheet,
    TouchableOpacity,
    View, Alert,
    PermissionsAndroid,
    ScrollView, Image,
    CameraRoll,
    Platform,
    AsyncStorage,
    BackHandler
} from 'react-native';
import { Icon, Text } from 'native-base';
import { RNCamera } from 'react-native-camera';
import { COLORS, FONTS, STRINGS } from '../../utils';

import Dialog from "react-native-dialog";

import { bindActionCreators } from "redux";
import { connect } from "react-redux";

import { savePOSM } from '../../redux/actions/ActionPOSM';

var moment = require('moment');

const { width, height } = Dimensions.get("window");

const CARD_WIDTH_2 = width / 3 - 50;
const CARD_HEIGHT_2 = CARD_WIDTH_2 - 30;

class POSMTakePhoto extends Component {

    constructor(props) {
        super(props);
        var type = props.navigation.getParam('type', '');
        this.state = {
            cameraType: 'back',
            type: type,
            dialogVisible: false,
            number: '',
            DATA: [],
            isChange: false
        }
    }

    componentWillMount = async () => {
        await this.requestCameraPermission();
        this.getDataSetUp();
    }

    componentDidMount = () => {
        BackHandler.addEventListener('hardwareBackPress', this.handleBackPress);
    }

    componentWillUnmount = () => {
        BackHandler.removeEventListener('hardwareBackPress', this.handleBackPress);
    }

    getDataSetUp = () => {

        const { type } = this.state;

        let _data = [];

        if (type === 'DEFAULT') {
            _data = [
                {
                    id: 0,
                    title: 'H. Tổng quan',
                    url: '',
                    type: type,
                    code: ''
                },
                {
                    id: 1,
                    title: 'H. Địa chỉ',
                    url: '',
                    type: type,
                    code: ''
                },
                {
                    id: 2,
                    title: '...',
                    url: '',
                    type: type,
                    code: ''
                }
            ];
        } else if (type === 'TRANH_PEPSI_AND_7UP') {
            _data = [
                {
                    id: 0,
                    title: '...',
                    url: '',
                    type: type,
                    code: ''
                },
                {
                    id: 1,
                    title: '...',
                    url: '',
                    type: type,
                    code: ''
                },
                {
                    id: 2,
                    title: '...',
                    url: '',
                    type: type,
                    code: ''
                }
            ];
        } else if (type === 'STORE_FAILED') {
            _data = [
                {
                    id: 0,
                    title: '...',
                    url: '',
                    type: type,
                    code: ''
                },
                {
                    id: 1,
                    title: '...',
                    url: '',
                    type: type,
                    code: ''
                },
                {
                    id: 2,
                    title: '...',
                    url: '',
                    type: type,
                    code: ''
                }
            ];
        }
        else {
            _data = [
                {
                    id: 0,
                    title: 'Ký PXN',
                    url: '',
                    type: type,
                    code: 'HINH_KY_PXN'
                },
                {
                    id: 1,
                    title: 'PXN đầy đủ',
                    url: '',
                    type: type,
                    code: 'HINH_PXN_DAYDU'
                },
                {
                    id: 2,
                    title: 'Ảnh SPVB',
                    url: '',
                    type: type,
                    code: 'HINH_SPVB'
                }
            ];
        }

        // ------ //

        const { dataResPOSM } = this.props;

        if (dataResPOSM.POSM != null && dataResPOSM.POSM.length != 0) {
            let items = _data;

            for (let i = 0; i < dataResPOSM.POSM.length; i++) {
                const x = dataResPOSM.POSM[i];

                if (x.Code === type) {
                    if (items[0].url == '' || items[0].url == null) {
                        items[0].url = x.Photo.uri;
                        continue;
                    }
                    else if (items[1].url == '' || items[1].url == null) {
                        items[1].url = x.Photo.uri;
                        continue;
                    }
                    else if (items[2].url == '' || items[2].url == null) {
                        items[2].url = x.Photo.uri;
                        continue;
                    }
                    else {
                        var itemAdd = {
                            id: items.length,
                            title: '...',
                            url: x.Photo.uri,
                            type: this.state.type,
                            code: ''
                        };

                        items.push(itemAdd);
                    }
                }
            }

            this.setState({ DATA: items });
        }
        else {
            this.setState({ DATA: _data });
        }
    }

    handleBackPress = async () => {

        const { isChange, type } = this.state;

        if (isChange) {
            Alert.alert(
                STRINGS.MessageTitleAlert, 'Bạn có muốn lưu dữ liệu thay đổi ?',
                [{
                    text: STRINGS.MessageActionOK, onPress: () => {
                        this.showDialog();
                        return true;
                    }
                },
                {
                    text: STRINGS.MessageActionCancel, onPress: () => {
                        this.props.navigation.state.params.refresh(type);
                        this.props.navigation.navigate('POSMOption');
                        return false;
                    }
                }],
                { cancelable: false }
            );
        }
        else {
            this.props.navigation.state.params.refresh(type);
            this.props.navigation.navigate('POSMOption');
            return false;
        }
    }

    requestCameraPermission = async () => {
        try {

            var per = [PermissionsAndroid.PERMISSIONS.CAMERA,
            PermissionsAndroid.PERMISSIONS.WRITE_EXTERNAL_STORAGE,
            PermissionsAndroid.PERMISSIONS.READ_EXTERNAL_STORAGE];
            const granted = await PermissionsAndroid.requestMultiple(per);

            if (granted === PermissionsAndroid.RESULTS.GRANTED) {
                console.log("You can use the camera")
            } else {
                console.log("Camera permission denied")
            }

        } catch (err) {
            console.warn(err)
        }
    }

    renderImageItemPOSM(item) {
        const { title, url, type, id } = item;

        return (
            <View style={styles.imgContainer}>
                <TouchableOpacity onLongPress={() => this.handleRemovePhoto(id)}>

                    {
                        (url != '' && url != null) ?
                            <View style={styles.rowIMG}>
                                <View style={styles.card2}>
                                    <Image
                                        source={{ uri: url }}
                                        style={styles.cardImage}
                                        resizeMode="cover"
                                    />
                                </View>
                            </View> :
                            <View style={styles.rowIMG}>
                                <View style={styles.card2}></View>
                            </View>
                    }

                </TouchableOpacity>
                <Text style={{ textAlign: 'center', fontSize: 10 }}>{title}</Text>
            </View >
        );
    }

    showDialog = () => {

        const { type, isChange } = this.state;

        if (type !== 'DEFAULT' && type !== 'STORE_FAILED' && isChange) {
            this.setState({ dialogVisible: true });
        }
        else {
            this.handleDoneData();
        }

    };

    handleCancel = () => {
        this.setState({ dialogVisible: false });
    };

    handleOk = () => {
        this.setState({ dialogVisible: false });

        const { number } = this.state;

        if (number != '' && number != null && number != 0) {
            this.handleDoneData();
        }
    };

    render() {

        const { cameraType, DATA, dialogVisible, number } = this.state;

        return (
            <View style={styles.con}>
                <Dialog.Container visible={dialogVisible}>
                    <Dialog.Title>Nhập số lượng</Dialog.Title>
                    <Dialog.Input keyboardType='number-pad'
                        style={styles.inputNum}
                        value={number}
                        onChangeText={x => {
                            if (!x.includes('.') && !x.includes(',') && !x.includes('-'))
                                this.setState({ number: x })
                        }
                        }>
                    </Dialog.Input>
                    <Dialog.Button label="Huỷ bỏ" onPress={this.handleCancel} />
                    <Dialog.Button label="Đồng ý" onPress={this.handleOk} />
                </Dialog.Container>
                <View
                    padder
                    style={styles.subContainer}>
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
                                onPress={this.handleBackPress}
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
                            <TouchableOpacity
                                onPress={this.showDialog}
                                style={styles.capture}>
                                <Icon name={'check'} size={15} type="FontAwesome"></Icon>
                            </TouchableOpacity>
                        </View>
                    </View>

                    <View style={styles.rowContainer2}>
                        <ScrollView
                            horizontal={true}
                            showsHorizontalScrollIndicator={false}
                            contentContainerStyle={{
                                height: CARD_HEIGHT_2 + 40
                            }}
                            style={{ padding: 0 }}>
                            {DATA.map((item, index) => {
                                return (
                                    this.renderImageItemPOSM(item)
                                );
                            })}
                        </ScrollView>
                    </View>

                </View>

            </View>
        );
    }

    takePicture = async function () {

        if (this.camera) {
            // const options = {
            //     quality: 1, base64: true, width: 1080, height: 1920,
            //     fixOrientation: true, forceUpOrientation: true, skipProcessing: true
            // };
            const options = {
                quality: 1, width: 1920, forceUpOrientation: true//, fixOrientation: true
            };
            // const options = {
            //     quality: 0.5, forceUpOrientation: true, fixOrientation: true
            // };
            const data = await this.camera.takePictureAsync(options);

            CameraRoll.saveToCameraRoll(data.uri, 'photo').then((res) => {
                const items = this.state.DATA;

                if (items[0].url == '' || items[0].url == null) {
                    items[0].url = res;
                }
                else if (items[1].url == '' || items[1].url == null) {
                    items[1].url = res;
                }
                else if (items[2].url == '' || items[2].url == null) {
                    items[2].url = res;
                }
                else {
                    var itemAdd = {
                        id: items.length,
                        title: '...',
                        url: res,
                        type: this.state.type,
                        code: ''
                    };
                    items.push(itemAdd);

                    this.setState({ DATA: items });
                }

                this.setState({ isChange: true });
                this.forceUpdate();
            });
        }
    };

    changeCameraType = () => {
        const { cameraType } = this.state;

        if (cameraType === 'back') {
            this.setState({ cameraType: 'front' });
        }
        else {
            this.setState({ cameraType: 'back' });
        }
    }

    handleDoneData = async () => {

        const { DATA, number, type } = this.state;
        const { dataResUser, dataResPOSM } = this.props;

        let numberPOSM = 0;
        if (type !== 'DEFAULT' && type !== 'STORE_FAILED') {
            numberPOSM = number;
        }

        try {
            if (dataResPOSM != null) {
                let _posmParse = dataResPOSM;

                var _dataSave = [];

                _posmParse.POSM.forEach(element => {
                    if (element.Code != type) {
                        _dataSave.push(element);
                    }
                });

                _posmParse.POSM = _dataSave;

                let listIMG = [];
                DATA.forEach(element => {
                    if (element.url !== '' && element.url !== null) {

                        listIMG.push(
                            {
                                Id: dataResUser.Data.Id,
                                Code: element.type,
                                Code2: element.code,
                                Date: moment().format('DD/MM/YYYY HH:mm:ss'),
                                Token: dataResUser.Data.Token,
                                TrackSessionId: _posmParse.TrackId,
                                PosmNumber: numberPOSM,
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

                _posmParse.POSM.push.apply(_posmParse.POSM, listIMG);
                this.props.savePOSM(_posmParse);
                this.props.navigation.state.params.refresh(type);

                setTimeout(() => {
                    this.props.navigation.navigate('POSMOption');
                }, 500);

            }
        } catch (error) {
            Alert.alert('Lỗi', error);
        }
    }

    handleRemovePhoto = (id) => {
        const items = this.state.DATA;
        items[id].url = '';
        this.setState({ isChange: true });
        this.forceUpdate();
    }
}

const styles = StyleSheet.create({
    con: {
        flex: 1
    },
    container: {
        flex: 1,
        flexDirection: 'column',
    },
    subContainer: {
        flex: 1,
        flexDirection: 'column',
        padding: 15,
        paddingTop: 0,
        paddingBottom: 0,
    },
    rowContainer2: {
        flexDirection: 'row',
    },
    imgContainer: {
        flexDirection: 'column',
        justifyContent: 'center'
    },
    rowIMG: {
        flexDirection: 'row',
    },
    bottomContainer: {
        flexDirection: 'column',
        justifyContent: 'center',
        alignContent: 'center',
        paddingBottom: 50
    },
    card2: {
        padding: 2,
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
    inputNum: {
        borderWidth: Platform.OS == 'android' ? 1 : 0,
        borderColor: Platform.OS == 'android' ? 'gray' : 'gray',
        borderRadius: Platform.OS == 'android' ? 5 : 0
    }
});

function mapStateToProps(state) {
    return {
        dataResUser: state.loginReducer.dataRes,
        dataResPOSM: state.POSMReducer.dataRes
    }
}

function dispatchToProps(dispatch) {
    return bindActionCreators({
        savePOSM
    }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(POSMTakePhoto);