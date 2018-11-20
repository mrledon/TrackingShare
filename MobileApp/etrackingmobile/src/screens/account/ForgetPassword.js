import React, { Component } from "react";
import { View, Alert, AsyncStorage, StyleSheet, Image } from "react-native";
import { Container, Input, Item, Text } from "native-base";
import Toast from 'react-native-root-toast';
import Spinner from 'react-native-loading-spinner-overlay';
import { KeyboardAwareScrollView } from 'react-native-keyboard-aware-scroll-view';
import { MainButton } from '../../components';
import { COLORS, FONTS, STRINGS } from '../../utils';

import { bindActionCreators } from "redux";
import { connect } from "react-redux";
import { fetchDataLogin } from '../../redux/actions/ActionLogin';

const LOGO = require('../../assets/images/logo.png');

class ForgetPassword extends Component {

  constructor(props) {
    super(props);
    this.state = {
      Email: '',
      Password: '',
      Message: '',
      isVisible: false
    }
  }

  // Reset data input
  reset() {
    this.setState({
      PhoneNumber: '',
      Password: '',
      Message: '',
      isVisible: false
    });
  }

  // Save data to local
  _storeData = async (_user, _password) => {
    try {
      await AsyncStorage.setItem('USER_SSC', _user);
      await AsyncStorage.setItem('PASSWORD_SSC', _password);
    } catch (error) {
      // Error saving data
    }
  }

  // Handle navigation
  handleFooter() {
    this.reset();
    this.props.navigation.navigate("Register");
  }

  // Handle login
  handleLogin() {

    this.props.navigation.navigate('Login');

    // let _phoneNumber = this.state.PhoneNumber;
    // let _password = this.state.Password;

    // // Check validate
    // if (_phoneNumber == '') {
    //   this.setState({ isVisible: true, Message: "Vui lòng nhập số điện thoại" });
    //   setTimeout(() => this.setState({
    //     isVisible: false
    //   }), 2000);
    //   return;
    // }

    // if (_password == '') {
    //   this.setState({ isVisible: true, Message: "Vui lòng nhập mật khẩu" });
    //   setTimeout(() => this.setState({
    //     isVisible: false
    //   }), 2000);
    //   return;
    // }

    // // Save user info on local 
    // this._storeData(_phoneNumber, _password);

    // // Call API
    // this.props.fetchDataLogin(_phoneNumber, _password)
    //   .then(() => setTimeout(() => {
    //     this.showAlert()
    //   }, 100));
  }

  // Handle status & result
  showAlert = () => {
    const { dataRes, error, errorMessage } = this.props;

    if (error) {
      let _mess = errorMessage + "";
      if (errorMessage == 'TypeError: Network request failed')
        _mess = "Vui lòng kiểm tra kết nối mạng";

      Alert.alert(
        'Thông báo', _mess,
        [{ text: 'OK', onPress: () => console.log('OK Pressed') }], { cancelable: false }
      );
      return;
    }
    else {
      if (dataRes.hasError == true) {
        Alert.alert(
          'Thông báo', dataRes.errorMessages + "",
          [{ text: 'OK', onPress: () => console.log('OK Pressed') }], { cancelable: false }
        );
      } else if (dataRes.hasError == false) {
        // this.reset();
        if (dataRes.userId == '00') { // don't have SSC Card
          this.props.setStatusUser('0');
          Alert.alert(
            'Thông báo', 'Bạn chưa có thẻ y tế (SSC Card), bạn có muốn đăng ký ngay?',
            [{ text: 'Hủy', onPress: () => console.log('OK Pressed') },
            { text: 'OK', onPress: () => this.props.navigation.navigate("HospitalInfo") }], { cancelable: false }
          );
        }
        else if (dataRes.userId == '01') // pending apply SSC Card
        {
          this.props.navigation.navigate("Pending");
        }
        else { // exist SSC Card
          this.props.setStatusUser('1');
          this.props.navigation.navigate("Home");
        }
      }
    }
  }

  // Render message
  renderMessage() {
    return (
      <Toast
        visible={this.state.isVisible}
        position={50}
        shadow={false}
        animation={false}
        hideOnPress={true}>
        {this.state.Message}
      </Toast>
    );
  }

  // Render
  render() {
    const { Email, Password } = this.state;

    return (
      <Container>
        {/* {this.renderMessage()} */}
        {/* <Spinner visible={this.props.isLoading} /> */}

        <KeyboardAwareScrollView
          enableAutomaticScroll={true}
          scrollEnabled={true}
          contentContainerStyle={styles.keyboardContainer}>
          <View padder style={styles.container}>
            <Image style={styles.logo} source={LOGO} />
            <Text
              style={styles.title}
              uppercase>
              {STRINGS.ForgetPasswordTitle}
            </Text>
            <Item regular style={styles.item}>
              <Input
                style={styles.input}
                placeholder={STRINGS.ForgetPasswordEmail}
                onChangeText={text => this.setState({ Email: text })}>
                {Email}
              </Input>
            </Item>
            <MainButton title={STRINGS.ForgetPasswordAction} onPress={() => this.handleLogin()}></MainButton>
          </View>
        </KeyboardAwareScrollView>

      </Container>
    );
  }
}

const styles = StyleSheet.create({
  keyboardContainer: {
    flexGrow: 1,
    justifyContent: 'center',
    alignItems: 'center'
  },
  container: {
    flex: 1,
    flexDirection: 'column',
    justifyContent: 'center',
    alignItems: 'center',
    maxWidth: 500,
    padding: 30
  },
  logo: {
    marginBottom: 50,
    width: 100,
    height: 100
  },
  title: {
    fontSize: 20,
    fontFamily: FONTS.MAIN_FONT_BOLD,
    marginBottom: 50,
    color: COLORS.BLUE_2E5665
  },
  item: {
    height: 40,
    marginBottom: 20
  },
  input: {
    fontFamily: FONTS.MAIN_FONT_REGULAR
  }
});

function mapStateToProps(state) {
  return {
    // isLoading: state.loginReducer.isLoading,
    // dataRes: state.loginReducer.dataRes,
    // error: state.loginReducer.error,
    // errorMessage: state.loginReducer.errorMessage
  }
}

function dispatchToProps(dispatch) {
  return bindActionCreators({
    fetchDataLogin
  }, dispatch);
}

export default connect(mapStateToProps, dispatchToProps)(ForgetPassword);
