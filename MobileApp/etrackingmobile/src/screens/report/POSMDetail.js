import React, { Component } from 'react';
import { View, StyleSheet, Image, TouchableOpacity, Dimensions, ScrollView, Alert } from 'react-native';
import { Text, Input, Item, Form, Textarea } from 'native-base';
import { MainButton, MainHeader } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import ImagePicker from 'react-native-image-picker';

// More info on all the options is below in the API Reference... just some common use cases shown here


const LOGO = require('../../assets/images/default.jpg');

const { width, height } = Dimensions.get("window");

// var base64Icon = 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAMAAAAoLQ9TAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAwBQTFRF7c5J78kt+/Xm78lQ6stH5LI36bQh6rcf7sQp671G89ZZ8c9V8c5U9+u27MhJ/Pjv9txf8uCx57c937Ay5L1n58Nb67si8tVZ5sA68tJX/Pfr7dF58tBG9d5e8+Gc6chN6LM+7spN1pos6rYs6L8+47hE7cNG6bQc9uFj7sMn4rc17cMx3atG8duj+O7B686H7cAl7cEm7sRM26cq/vz5/v767NFY7tJM78Yq8s8y3agt9dte6sVD/vz15bY59Nlb8txY9+y86LpA5LxL67pE7L5H05Ai2Z4m58Vz89RI7dKr+/XY8Ms68dx/6sZE7sRCzIEN0YwZ67wi6rk27L4k9NZB4rAz7L0j5rM66bMb682a5sJG6LEm3asy3q0w3q026sqC8cxJ6bYd685U5a457cIn7MBJ8tZW7c1I7c5K7cQ18Msu/v3678tQ3aMq7tNe6chu6rgg79VN8tNH8c0w57Q83akq7dBb9Nld9d5g6cdC8dyb675F/v327NB6////AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA/LvB3QAAAMFJREFUeNpiqIcAbz0ogwFKm7GgCjgyZMihCLCkc0nkIAnIMVRw2UhDBGp5fcurGOyLfbhVtJwLdJkY8oscZCsFPBk5spiNaoTC4hnqk801Qi2zLQyD2NlcWWP5GepN5TOtSxg1QwrV01itpECG2kaLy3AYiCWxcRozQWyp9pNMDWePDI4QgVpbx5eo7a+mHFOqAxUQVeRhdrLjdFFQggqo5tqVeSS456UEQgWE4/RBboxyC4AKCEI9Wu9lUl8PEGAAV7NY4hyx8voAAAAASUVORK5CYII=';


const CARD_WIDTH = width / 2 - 40;
const CARD_HEIGHT = CARD_WIDTH - 20;

const CARD_WIDTH_2 = width / 3 - 20;
const CARD_HEIGHT_2 = CARD_WIDTH_2 - 10;

class POSMDetail extends Component {
    constructor(props) {
        super(props);
        this.state = {
            base64Icon: 'dffsd'
        };
    }

    handleTakePhoto() {
        // this.props.navigation.navigate('TakePhoto');

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
                Alert.alert(response.uri);
                //   const source = { uri: response.uri };

                //   // You can also display the image using data:
                //   // const source = { uri: 'data:image/jpeg;base64,' + response.data };

                this.setState({
                    base64Icon: response.uri,
                });
            }
        });
    }

    handleBack = () => {
        this.props.navigation.navigate('Home');
    }

    renderImageItemStore(title) {

        return (
            <TouchableOpacity onPress={this.handleTakePhoto.bind(this)}>
                <View style={styles.imgContainer}>
                    <View style={styles.card}>
                        <Image
                            // source={{ uri: this.state.base64Icon }}
                            source={LOGO}
                            style={styles.cardImage}
                            resizeMode="cover"
                        />
                    </View>
                    <Text style={{ textAlign: 'center' }}>{title}</Text>
                </View>
            </TouchableOpacity>
        );
    }

    renderImageItemPOSM(title) {
        return (
            <TouchableOpacity onPress={this.handleTakePhoto.bind(this)}>
                <View style={styles.imgContainer}>
                    <View style={styles.card2}>
                        <Image
                            source={LOGO}
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
                            // justifyContent: 'center',
                            marginBottom: 50
                        }}
                        style={{ padding: 10 }}>

                        <MainButton
                            icon={'arrow-down'}
                            isIcon={true}
                            style={styles.button}
                            title={'Loại hình cửa hàng'}
                            onPress={() => this.handlePOSMPress()} />

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreName}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <Item regular style={styles.item}>
                                    <Input
                                        style={styles.input}
                                        onChangeText={text => this.setState({ Email: text })}>
                                    </Input>
                                </Item>
                            </View>
                        </View>

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreNumber}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <Item regular style={styles.item}>
                                    <Input
                                        style={styles.input}
                                        onChangeText={text => this.setState({ Email: text })}>
                                    </Input>
                                </Item>
                            </View>
                        </View>

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreStreet}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <Item regular style={styles.item}>
                                    <Input
                                        style={styles.input}
                                        onChangeText={text => this.setState({ Email: text })}>
                                    </Input>
                                </Item>
                            </View>
                        </View>

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreWard}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <Item regular style={styles.item}>
                                    <Input
                                        style={styles.input}
                                        onChangeText={text => this.setState({ Email: text })}>
                                    </Input>
                                </Item>
                            </View>
                        </View>

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreDistrict}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <Item regular style={styles.item}>
                                    <Input
                                        style={styles.input}
                                        onChangeText={text => this.setState({ Email: text })}>
                                    </Input>
                                </Item>
                            </View>
                        </View>

                        <View style={styles.rowContainer}>
                            <View style={styles.leftItem}>
                                <Text style={styles.title}>{STRINGS.POSMDetailTitleStoreCity}</Text>
                            </View>
                            <View style={styles.rightItem}>
                                <Item regular style={styles.item}>
                                    <Input
                                        style={styles.input}
                                        onChangeText={text => this.setState({ Email: text })}>
                                    </Input>
                                </Item>
                            </View>
                        </View>

                        <View style={styles.forgetContainer}>
                            <Text
                                onPress={() => this.props.navigation.navigate("ForgetPassword")}
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
                                this.renderImageItemStore('Hình 1')
                            }
                            {
                                this.renderImageItemStore('Hình 2')
                            }
                        </View>

                        <View style={styles.line} />

                        <View style={styles.centerContainer}>
                            <Text style={styles.itemSubTitle} uppercase>{STRINGS.POSMDetailSubTitle2}</Text>
                        </View>

                        <MainButton
                            icon={'arrow-down'}
                            isIcon={true}
                            style={styles.button}
                            title={'Chọn loại POSM'}
                            onPress={() => this.handlePOSMPress()} />

                        <View style={styles.rowContainer2}>
                            {
                                this.renderImageItemPOSM('Hình 1')
                            }
                            {
                                this.renderImageItemPOSM('Hình 2')
                            }
                            {
                                this.renderImageItemPOSM('Hình 3')
                            }
                        </View>

                        <View style={styles.rowContainer2}>
                            {
                                this.renderImageItemPOSM('*')
                            }
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
                            onPress={() => this.handlePOSMPress()} />

                    </ScrollView>

                    {/* {this.state.list.map((item, index) => {
                        return (
                            <MainButton
                                style={styles.button}
                                title={item.title}
                                onPress={() => this.handlePOSMPress()} />
                        );
                    })} */}

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
        alignItems: 'center',
        justifyContent: 'center',
        flex: 1,
        flexDirection: 'row',
        paddingBottom: 10
    },
    imgContainer: {
        flexDirection: 'column',
        justifyContent: 'center'
    },
    title: {
        // margin: 5
    },
    bottomContainer: {
        flexDirection: 'column',
        justifyContent: 'center',
        alignContent: 'center',
        paddingBottom: 50
    },
    button: {
        height: 50,
        marginBottom: 20,
        width: 250
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
        padding: 10,
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
        alignContent: 'center',
    },
    rightItem: {
        flex: 0.6,
        justifyContent: 'center',
        alignContent: 'center',
    },
    item: {
        height: 40,
        marginBottom: 20
    },
    input: {
        fontFamily: FONTS.MAIN_FONT_REGULAR
    },
    forgetContainer: {
        alignItems: 'flex-end',
        alignSelf: 'stretch',
    },
    forgetText: {
        color: COLORS.BLUE_2E5665,
        fontFamily: FONTS.MAIN_FONT_BOLD,
    }
});

export default POSMDetail;
