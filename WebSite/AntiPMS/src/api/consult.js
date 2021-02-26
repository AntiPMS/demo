import Axios from 'axios'

let axios = Axios.create({
  // baseURL: process.env.NODE_ENV === 'development' ? '/development' : 'http://cloudhospital.knjs.com.cn', // url = base url + request url
  baseURL: 'https://api.qinko.club', // url = base url + request url
  withCredentials: false, // send cookies when cross-domain requests
  timeout: 5000 // request timeout
})

export function GetHisInformation (lastSendDate, targetId, num) {
  return axios.get('/api/Users/GetUserHisChatMsgBySendDateDecreasing?lastSendDate=' + lastSendDate + '&targetId=' + targetId + '&num=' + num)
}
