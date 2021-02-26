import Axios from 'axios'
import Vue from 'vue'

// create an axios instance
const axios = Axios.create({
  baseURL: 'https://api.qinko.club', // url = base url + request url
  withCredentials: false, // send cookies when cross-domain requests
  timeout: 5000 // request timeout
})

// 添加请求拦截器
axios.interceptors.request.use(
  config => {
    // 在发送请求之前做些什么
    // if (store.getters.token) {
    // let each request carry token
    // ['X-Token'] is a custom headers key
    // please modify it according to the actual situation
    // config.headers['X-Token'] = store.getters.token;
    config.headers['userId'] = 'qinko'
    // }
    return config
  },
  error => {
    // 对请求错误做些什么
    console.log(error) // for debug
    return Promise.reject(error)
  }
)

// response interceptor
axios.interceptors.response.use(
  /**
     * If you want to get http information such as headers or status
     * Please return  response => response
     */

  /**
     * Determine the request status by custom code
     * Here is just an example
     * You can also judge the status by HTTP Status Code
     */
  response => {
    // 对响应数据做点什么
    const res = response.data

    // if the custom code is not 20000, it is judged as an error.
    if (res.resultStatus !== 200) {
      if (res.resultStatus !== 301) {
        // 请求失败
        Vue.prototype.$dialog.alert({
          message: res.msg || 'error' // 提示内容
          // duration: 5 * 1000 //自动关闭的延时，单位秒，不关闭可以写 0
        })
      } else {
        Vue.prototype.$toast('未查询到数据')
      }
      return Promise.reject(res.msg || 'error')
    } else {
      // 请求成功
      return res.resultData
    }
  },
  error => {
    // 对响应错误做点什么
    console.log('err' + error) // for debug
    Vue.prototype.$dialog.alert({
      message: error.message // 提示内容
      // duration: 5 * 1000 //自动关闭的延时，单位秒，不关闭可以写 0
    })
    return Promise.reject(error)
  }
)

export default axios
