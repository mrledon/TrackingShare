import React, { Component } from 'react';
import {
  AppRegistry,
  Dimensions,
  StyleSheet,
  TouchableOpacity,
  View, Alert,
  PermissionsAndroid,
  ScrollView, Image,
  Platform,
  CameraRoll
} from 'react-native';
import { Icon, Text } from 'native-base';
import { RNCamera } from 'react-native-camera';
import { COLORS, FONTS, STRINGS } from '../../utils';
import { MainButton, MainHeader } from '../../components';

import RNFetchBlob from 'rn-fetch-blob';

const PictureDir = RNFetchBlob.fs.dirs.PictureDir;
const fs = RNFetchBlob.fs;


const { width, height } = Dimensions.get("window");

const CARD_WIDTH = width / 2 - 40;
const CARD_HEIGHT = CARD_WIDTH - 20;

const CARD_WIDTH_2 = width / 3 - 20;
const CARD_HEIGHT_2 = CARD_WIDTH_2 - 10;

export default class TakePhoto extends Component {

  constructor(props) {
    super(props);
    this.state = {
      cameraType: 'back',
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
    }
  }

  componentWillMount(){
    this.requestCameraPermission();
  }

  componentDidMount() {
    // this.requestCameraPermission();
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

          {
            (url != '' && url != null) ?
              <TouchableOpacity onPress={() => this.handleRemovePhoto(type, id)}>
                <View style={styles.iconDelete}>
                  <Icon name={'times'} type="FontAwesome" size={10}></Icon>
                </View>
              </TouchableOpacity> : <View />
          }
          {
            (url != '' && url != null) ?
              // <TouchableOpacity onPress={() => this.handleTakePhoto(type, id)}>
                <View style={styles.card2}>
                  <Image
                    source={{ uri: url }}
                    style={styles.cardImage}
                    resizeMode="cover"
                  />
                </View>:
              // </TouchableOpacity> :
              // <TouchableOpacity onPress={() => this.handleTakePhoto(type, id)}>
                <View style={styles.card2}>
                </View>
              // </TouchableOpacity>
          }

        </View>
        <Text style={{ textAlign: 'center' }}>{title}</Text>
      </View >
    );
  }

  render() {

    const { cameraType, STICKER_7UP } = this.state;

    return (
      <View style={styles.con}>
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
            {/* <MainButton
              style={styles.button2}
              title={'Chụp ảnh'}
              onPress={this.addTRANH_PEPSI_AND_7UP} /> */}
          </ScrollView>
        </View>
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
          <View style={{ flex: 0, flexDirection: 'row', justifyContent: 'center', margin: 5 }}>
            {/* <TouchableOpacity
              onPress={this.handleCamBack}
              style={styles.capture}>
              <Icon name={'chevron-left'} size={15} type="FontAwesome"></Icon>
            </TouchableOpacity> */}
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

        const items = this.state.STICKER_7UP;
        items[0].url = imageLocation;

        this.forceUpdate();
      }
      else {
        CameraRoll.saveToCameraRoll(data.uri, 'photo').then((res) => {
          console.log('res ne', res)
          const items = this.state.STICKER_7UP;

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
                type: 'STICKER_7UP',
                code: ''
              };
              items.push(itemAdd);

              this.setState({ STICKER_7UP: items });

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
}

const styles = StyleSheet.create({
  con: {
    flex: 1,
    flexDirection: 'column',
    marginTop: 15
  },
  container: {
    flex: 1,
    flexDirection: 'column',
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
    flexDirection: 'row',
    paddingBottom: 10
  },
  imgContainer: {
    flexDirection: 'column',
    justifyContent: 'center'
  },
  rowIMG: {
    flexDirection: 'row',
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
    marginTop: 6
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
    height: CARD_HEIGHT - 30,
    width: CARD_WIDTH - 30,
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
    padding: 10,
    // paddingHorizontal: 20,
    alignSelf: 'center',
    marginHorizontal: 20,
    marginVertical: 10,
    width: 55,
    alignItems: 'center'
  },
  iconDelete: {
    // position: 'absolute',
    // left: 10,
    // top: 5
    borderColor: 'black',
    borderRadius: 50,
    borderWidth: 1,
    paddingHorizontal: 7,
    paddingVertical: 4
  }
});