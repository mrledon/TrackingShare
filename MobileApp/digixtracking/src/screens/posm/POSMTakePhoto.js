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
    AsyncStorage
} from 'react-native';
import { Icon, Text } from 'native-base';
import { RNCamera } from 'react-native-camera';
import { COLORS, FONTS, STRINGS } from '../../utils';
import { MainButton, MainHeader } from '../../components';

import RNFetchBlob from 'rn-fetch-blob';
import Dialog from "react-native-dialog";

import { bindActionCreators } from "redux";
import { connect } from "react-redux";

const PictureDir = RNFetchBlob.fs.dirs.PictureDir;
const fs = RNFetchBlob.fs;
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
            DATA: [
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
            ],
        }
    }

    componentWillMount = async () => {
        this.requestCameraPermission();
    }

    requestCameraPermission = async () => {
        try {
            const granted = await PermissionsAndroid.request(
                PermissionsAndroid.PERMISSIONS.WRITE_EXTERNAL_STORAGE,
                {
                    'title': 'DigiX Tracking App Camera, Storage Permission',
                    'message': 'DigiX Tracking App needs access to your camera and storage' +
                        'so you can take awesome pictures.'
                }
            )
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
                <View style={styles.rowIMG}>
                    <View style={styles.card2}>
                        <Image
                            source={{ uri: url }}
                            style={styles.cardImage}
                            resizeMode="cover"
                        />
                    </View>
                </View>
                <Text style={{ textAlign: 'center', fontSize: 10 }}>{title}</Text>
            </View >
        );
    }

    showDialog = () => {
        this.setState({ dialogVisible: true });
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
                        value={number}
                        onChangeText={number => this.setState({ number })}>
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
                                onPress={this.takePicture.bind(this)}
                                style={styles.capture}>
                                <Icon name={'camera'} size={15} type="FontAwesome"></Icon>
                            </TouchableOpacity>
                            <TouchableOpacity
                                onPress={this.changeCameraType}
                                style={styles.capture}>
                                <Icon name={'refresh'} size={15} type="FontAwesome"></Icon>
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

        let imageLocation = PictureDir + '/' + 'img.jpg';

        if (this.camera) {
            const options = {
                quality: 1, base64: true, width: 1080, height: 1920,
                fixOrientation: true, forceUpOrientation: true, skipProcessing: true
            };
            const data = await this.camera.takePictureAsync(options);

            if (Platform == 'android') {
                fs.createFile(imageLocation, data.uri, 'uri');

                const items = this.state.DATA;
                items[0].url = imageLocation;

                this.forceUpdate();
            }
            else {
                CameraRoll.saveToCameraRoll(data.uri, 'photo').then((res) => {
                    const items = this.state.DATA;

                    for (let index = 0; index < items.length; index++) {
                        const element = items[index];
                        if (element.url == '' || element.url == null) {
                            items[index].url = res;
                            break;
                        }

                        if (index == items.length - 1 && (element.url != '' && element.url != null)) {

                            var itemAdd = {
                                id: items.length,
                                title: '...',
                                url: '',
                                type: this.state.type,
                                code: ''
                            };
                            items.push(itemAdd);

                            this.setState({ DATA: items });

                        }
                    }

                    this.forceUpdate();
                });
            }
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

        const { DATA, number } = this.state;
        const { dataResUser } = this.props;

        try {
            const posm = await AsyncStorage.getItem('POSM_SSC');

            if (posm != null) {
                let _posmParse = JSON.parse(posm);

                let listIMG = [];
                DATA.forEach(element => {
                    if (element.url !== '' && element.url !== null) {

                        listIMG.push(
                            {
                                Id: dataResUser.Data.Id,
                                Code: element.type,
                                Code2: element.code,
                                Date: moment().format('DD/MM/YYYY'),
                                Token: dataResUser.Data.Token,
                                TrackSessionId: _posmParse.TrackId,
                                PosmNumber: number,
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
                await AsyncStorage.setItem('POSM_SSC', JSON.stringify(_posmParse));

                console.log('dataaaaaaaa', _posmParse)

                setTimeout(() => { this.props.navigation.navigate('POSMOption') }, 1000);
            }
        } catch (error) {
            Alert.alert('Lỗi', error);
        }
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
        marginTop: Platform.OS == 'android' ? 50 : 30,
        marginBottom: Platform.OS == 'android' ? 20 : 0
    },
    capture: {
        flex: 0,
        backgroundColor: '#fff',
        borderRadius: 5,
        padding: 10,
        alignSelf: 'center',
        marginHorizontal: 20,
        marginVertical: 5,
        alignItems: 'center',
        width: 55
    }
});

function mapStateToProps(state) {
    return {
        dataResUser: state.loginReducer.dataRes
    }
}

function dispatchToProps(dispatch) {
    return bindActionCreators({
    }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(POSMTakePhoto);