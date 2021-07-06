<template>
  <div style="overflow-y: scroll;">

    <div :style="height"
         style="overflow-y: scroll;"
         id="dhk">
      <van-cell v-for="(msgtext,index) in receiveMsgText"
                :key="index">
        <a v-if="msgtext.msgtype==2 && msgtext.sendid !=sendMsg.senderId">
          <img src="../assets/医生头像@3x.png"
               style="float: left;margin-top: 15px;width:44px;height: 44px">
          <div style="float: left;">
            <label style="color:#3f9775;font-size:12px;margin: 0 5px;">{{msgtext.senderName}}</label>
            <br>
            <img :src=msgtext.msg
                 style="max-width: 200px;max-height:200px"
                 :id=index
                 :onerror="defaultImg"
                 :key="index2"
                 @click="getImg(msgtext.msg, msgtext.imageIndex)">
          </div>
        </a>
        <a v-if="msgtext.msgtype==1 && msgtext.sendid !=sendMsg.senderId">
          <img src="../assets/医生头像@3x.png"
               style="float: left;margin-top: 10px;width:44px;height: 44px">
          <div style="float: left">
            <label style="color:#3f9775;font-size: 12px; margin: 0 5px;">{{msgtext.senderName}}</label>
            <br>
            <p class="doctorCommunicationInputs"
               :style='{width:msgtext.msg.length*15+"px"}'>
              {{ msgtext.msg }}
            </p>
          </div>
        </a>
        <a v-if="msgtext.msgtype==1 && msgtext.sendid ==sendMsg.senderId">
          <img src="../assets/病人头像@3x.png"
               style="float: right;margin-top: 15px;width:44px;">
          <div style="float: right;text-align: right">
            <label style="color:#3f9775;font-size: 12px; margin: 0 5px;">{{msgtext.senderName}}</label>
            <p class="customerCommunicationInputs"
               style="text-align: right"
               :style='{width:msgtext.msg.length*15+"px"}'>
              {{ msgtext.msg }}
            </p>
          </div>
        </a>
        <a v-if="msgtext.msgtype==2 && msgtext.sendid ==sendMsg.senderId">
          <img src="../assets/病人头像@3x.png"
               style="float: right;margin-top: 10px;width:44px;">
          <div style="float: right">
            <label style="float: right;padding-right: 5px;color:#3f9775;font-size: 12px">{{msgtext.senderName}}</label>
            <br>
            <img :src=msgtext.msg
                 style="max-width: 200px;margin: 0 5px;max-height:200px"
                 :id=index
                 :onerror="defaultImg"
                 @click="getImg(msgtext.msg, msgtext.imageIndex)"
                 :key="index2">
          </div>
        </a>
        <div style="clear: both;">
          <p style="color: #dfdfdf;left: 0;right: 0;text-align: center"
             v-if="PD(index)!='0'">
            {{ PD(index) }}
          </p>
          <p v-else>
          </p>
        </div>
      </van-cell>
    </div>

    <div style="margin-bottom:5px;background: #ffffff;position: fixed;bottom: 0">
      <van-field class="communicationInput"
                 v-model="sendMsg.msg"
                 v-on:keyup.enter="doSend"
                 maxlength="1000"
                 style="float: left;"
                 :style="{width:KD}" />
      <van-col>
        <van-uploader :after-read="afterRead">
          <van-icon name="photo-o"
                    size="35px"
                    style='padding-top:3px' />
        </van-uploader>
      </van-col>
      <van-button v-on:click="doSend"
                  type="primary"
                  style="float:right;margin-left: 15px;margin-top: 7px;height:28px">发送</van-button>
    </div>
  </div>
</template>

<script>
import { ImagePreview } from 'vant'
import { GetHisInformation } from '@/api/consult.js'

export default {
  name: 'MyWebSocket',
  components: {
  },
  data () {
    return {
      jsq: 0,
      currentMsg: '',
      container: null,
      isEnterSend: true,
      socket: null,
      receiveMsgText: [],
      sendMsg: {
        Id: '',
        senderId: '',
        senderName: '',
        targetId: '',
        msgType: 1,
        msg: '',
        key: '0'
      },
      KD: '',
      height: {
        height: ''
      },
      width: {
        width: ''
      },
      timeoutObj: undefined,
      serverTimeoutObj: undefined,
      i: 0,
      lock: true,
      locks: true,
      listData: [], // 图片数组
      index2: 0,
      doctor: ''
    }
  },
  created: function () {
    let id = this.guid()
    this.sendMsg.senderId = 'user-' + id
    this.sendMsg.senderName = '用户-' + id
    this.sendMsg.targetId = 'qinko'
    this.onLoadMsg()
    this.hh()
  },
  destroy: function () {
    this.socket.close()
  },
  methods: {
    defaultImg (e) {
      console.log(e.parent())
    },
    mounted () {
      let that = this
      that.lock = true
      that.locks = true
    },
    getImg (images, index) {
      console.log(images)
      ImagePreview({
        images: this.listData,
        showIndex: true,
        loop: false,
        startPosition: index
      })
    },

    afterRead (file) {
      this.sendMsg.msgType = 2
      this.sendMsg.msg = file.content
      this.socket.send(JSON.stringify(this.sendMsg))

      var _this = this
      let yy = new Date().getFullYear()
      var mm =
        new Date().getMonth() < 10
          ? '0' + (new Date().getMonth() + 1)
          : new Date().getMonth() + 1
      var dd =
        new Date().getDate() < 10
          ? '0' + new Date().getDate()
          : new Date().getDate()
      let hh = new Date().getHours()
      let mf =
        new Date().getMinutes() < 10
          ? '0' + new Date().getMinutes()
          : new Date().getMinutes()
      let ss =
        new Date().getSeconds() < 10
          ? '0' + new Date().getSeconds()
          : new Date().getSeconds()
      _this.gettime = yy + '-' + mm + '-' + dd + ' ' + hh + ':' + mf + ':' + ss
      this.listData.push(file.content)
      let imageIndex = this.listData.length - 1
      this.receiveMsgText.push(
        {
          msg: file.content,
          msgtype: '2',
          senderName: this.sendMsg.senderName,
          sendid: this.sendMsg.senderId,
          sendDate: _this.gettime,
          imageIndex: imageIndex
        })
      this.sendMsg.msg = ''
      clearTimeout(this.timeOut)
      this.timeOut = setTimeout(() => {
        var div = document.getElementById('dhk')
        div.scrollTop = (div.scrollHeight)
      }, 100)
    },
    onLoadMsg () {
      this.doConnect(this)
    },
    hh () {
      this.height.height = document.documentElement.clientHeight - 100 + 'px'
      this.width.width = document.documentElement.clientWidth - 120 + 'px'
      this.KD = document.documentElement.clientWidth - 120 + 'px'
    },
    guid () {
      function S4 () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1)
      }
      return (S4() + S4())
    },
    heartbeat () {
      var self = this
      this.timeoutObj && clearTimeout(this.timeoutObj)
      this.serverTimeoutObj && clearTimeout(this.serverTimeoutObj)
      self.socket.send(JSON.stringify({
        msg: '',
        msgtype: '6',
        senderName: self.sendMsg.senderName,
        sendid: self.sendMsg.senderId,
        targetId: self.targetId,
        id: ''
      }))
    },
    doConnect (app) {
      let uri = 'wss://api.qinko.club/ws?senderId=' + this.sendMsg.senderId + '&targetId=' + this.sendMsg.targetId

      this.socket = new WebSocket(uri)
      let that = this
      this.socket.onopen = function () {
        setInterval(() => {
          that.heartbeat()
        }, 50000)
        var _this = this
        let yy = new Date().getFullYear()
        var mm =
          new Date().getMonth() < 10
            ? '0' + (new Date().getMonth() + 1)
            : new Date().getMonth() + 1
        var dd =
          new Date().getDate() < 10
            ? '0' + new Date().getDate()
            : new Date().getDate()
        let hh = new Date().getHours()
        let mf =
          new Date().getMinutes() < 10
            ? '0' + new Date().getMinutes()
            : new Date().getMinutes()
        let ss =
          new Date().getSeconds() < 10
            ? '0' + new Date().getSeconds()
            : new Date().getSeconds()
        _this.gettime = yy + '-' + mm + '-' + dd + ' ' + hh + ':' + mf + ':' + ss
        if (app.locks === true) {
          GetHisInformation(_this.gettime, app.sendMsg.targetId, 20).then(
            (result) => {
              if (result.data.resultStatus === 200) {
                for (let i = 0; i < result.data.resultData.length; i++) {
                  if (result.data.resultData[i].msgType === 2) {
                    app.listData.push(result.data.resultData[i].msg)
                    result.data.resultData[i].imageIndex = app.listData.length - 1
                  }
                  app.receiveMsgText.push({
                    msg: result.data.resultData[i].msg,
                    msgtype: result.data.resultData[i].msgType,
                    sendid: result.data.resultData[i].senderId,
                    sendDate: result.data.resultData[i].sendDate,
                    imageIndex: result.data.resultData[i].imageIndex,
                    senderName: result.data.resultData[i].senderName
                  })
                }
              }
            },
            () => {
            }
          )
          app.locks = false
          clearTimeout(app.timeOut)
          app.timeOut = setTimeout(() => {
            var div = document.getElementById('dhk')
            div.scrollTop = (div.scrollHeight)
          }, 1000)

          setTimeout(() => {
            var div = document.getElementById('dhk')
            div.scrollTop = (div.scrollHeight)
            document.getElementById('dhk').addEventListener('scroll', function () {
              if (div.scrollTop === '0' && app.lock === true) {
                GetHisInformation(app.receiveMsgText[0].sendDate, app.sendMsg.targetId, 5).then(
                  (result) => {
                    if (result.data.resultStatus === 200) {
                      for (let i = result.data.resultData.length - 1; i >= 0; i--) {
                        if (result.data.resultData[i].msgType === 2) {
                          app.listData.push(result.data.resultData[i].msg)
                          result.data.resultData[i].imageIndex = app.listData.length - 1
                        }
                        app.receiveMsgText.unshift({
                          msg: result.data.resultData[i].msg,
                          msgtype: result.data.resultData[i].msgType,
                          sendid: result.data.resultData[i].senderId,
                          sendDate: result.data.resultData[i].sendDate,
                          imageIndex: result.data.resultData[i].imageIndex,
                          senderName: result.data.resultData[i].senderName
                        })
                      }
                    }
                  },
                  () => { }
                )
                app.lock = false
              }
            })
          }, 2000)
          setInterval(() => {
            app.lock = true
          }, 3000)
        }
      }
      this.socket.onclose = function () {
        app.receiveMsgText.push({
          isSelf: false,
          senderName: 'system',
          msg: '连接已断开'
        })
        if (that.i <= 5) {
          that.socket = new WebSocket(uri)
          that.i++
        }
      }
      this.socket.onmessage = function (e) {
        var rmsg = JSON.parse(e.data)
        app.receiveMsgText.push(
          {
            sendid: rmsg['SenderId'],
            senderName: rmsg['SenderName'],
            msgtype: rmsg['MsgType'],
            msg: rmsg['Msg'],
            sendDate: rmsg['SendDate']
          })
        clearTimeout(this.timeOut)
        this.timeOut = setTimeout(() => {
          var div = document.getElementById('dhk')
          div.scrollTop = (div.scrollHeight)
        }, 100)
      }
      this.socket.onerror = function () {
        app.receiveMsgText.push({
          isSelf: false,
          senderName: 'system',
          msg: '连接已断开'
        })
        if (that.i <= 5) {
          that.socket = new WebSocket(uri)
          that.i++
        }
      }
    },
    doSend () {
      if (this.sendMsg.msg !== '') {
        var _this = this
        let yy = new Date().getFullYear()
        var mm =
          new Date().getMonth() < 10
            ? '0' + (new Date().getMonth() + 1)
            : new Date().getMonth() + 1
        var dd =
          new Date().getDate() < 10
            ? '0' + new Date().getDate()
            : new Date().getDate()
        let hh = new Date().getHours()
        let mf =
          new Date().getMinutes() < 10
            ? '0' + new Date().getMinutes()
            : new Date().getMinutes()
        let ss =
          new Date().getSeconds() < 10
            ? '0' + new Date().getSeconds()
            : new Date().getSeconds()
        _this.gettime = yy + '-' + mm + '-' + dd + ' ' + hh + ':' + mf + ':' + ss

        this.sendMsg.msgType = 1
        this.socket.send(JSON.stringify(this.sendMsg))
        console.log(JSON.stringify(this.sendMsg))
        this.receiveMsgText.push(
          {
            senderName: this.sendMsg.senderName,
            msg: this.sendMsg.msg,
            msgtype: '1',
            sendid: this.sendMsg.senderId,
            sendDate: _this.gettime
          })
        this.sendMsg.msg = ''
        setTimeout(() => {
          var div = document.getElementById('dhk')
          div.scrollTop = (div.scrollHeight)
        }, 100)
      }
    },
    // 时间判断
    PD (index) {
      if (index !== '0') {
        if (index !== this.receiveMsgText.length - 1) {
          var _this = this
          let yy = new Date().getFullYear()
          var mm =
            new Date().getMonth() < 10
              ? '0' + (new Date().getMonth() + 1)
              : new Date().getMonth() + 1
          var dd =
            new Date().getDate() < 10
              ? '0' + new Date().getDate()
              : new Date().getDate()
          _this.gettime = yy + '-' + mm + '-' + dd
          let dat = this.receiveMsgText[index].sendDate.substring(0, 10) + ' ' + this.receiveMsgText[index].sendDate.substring(11, 19)
          if (this.receiveMsgText[index - 1]) {
            let myDate = this.receiveMsgText[index - 1].sendDate.substring(0, 10) + ' ' + this.receiveMsgText[index - 1].sendDate.substring(11, 19)
            if (this.getTimedata(dat, myDate) >= 600 && this.receiveMsgText[index].sendDate.substring(0, 10) === _this.gettime) {
              return this.receiveMsgText[index].sendDate.substring(11, 16)
            } else if (this.getTimedata(dat, myDate) >= 600 && this.receiveMsgText[index].sendDate.substring(0, 10) !== _this.gettime) {
              return this.receiveMsgText[index].sendDate.substring(0, 10) + ' ' + this.receiveMsgText[index].sendDate.substring(11, 16)
            }
          }
        }
      } else {
        return ''
      }
    },

    getTimedata (myDate, dat) {
      let getYeardata = dat.split(' ')[0]
      let getTimedata = dat.split(' ')[1]
      let beforeYear = getYeardata.split('-')[0]
      let beforeMonth = getYeardata.split('-')[1]
      let beforeDate = getYeardata.split('-')[2]
      let beforeHours = getTimedata.split(':')[0]
      let beforeMinutes = getTimedata.split(':')[1]
      let beforeSeconds = getTimedata.split(':')[2]
      let getYeardatas = myDate.split(' ')[0]
      let getTimedatas = myDate.split(' ')[1]
      let afterYear = getYeardatas.split('-')[0]
      let afterMonth = getYeardatas.split('-')[1]
      let afterDate = getYeardatas.split('-')[2]
      let afterHours = getTimedatas.split(':')[0]
      let afterMinutes = getTimedatas.split(':')[1]
      let afterSeconds = getTimedatas.split(':')[2]

      // 计算差值
      let getDifference = (afterYear - beforeYear) * 365 * 24 * 60 * 60
      getDifference += (afterMonth - beforeMonth) * 30 * 24 * 60 * 60
      getDifference += (afterDate - beforeDate) * 24 * 60 * 60
      getDifference += (afterHours - beforeHours) * 60 * 60
      getDifference += (afterMinutes - beforeMinutes) * 60
      getDifference += (afterSeconds - beforeSeconds)

      return getDifference
    }
  }
}
</script>
<style lang="less">
.communicationInput input {
  border: 2px solid #3f9775;
  padding: 0 5px;
}
</style>
<style scoped lang="less">
@import "../style/var.less";
.van-cell::after {
  border-bottom: 0;
}
.doctorCommunicationInputs {
  background-color: @white;
  color: @green;
  padding: 5px 10px;
  border-radius: 8px;
  box-shadow: 0px 0px 5px 0px @gray-5;
  margin: 5px;
  max-width: 65vw;
}
.customerCommunicationInputs {
  background-color: @green;
  color: @white;
  padding: 5px 10px;
  border-radius: 8px;
  margin: 0 5px;
  max-width: 65vw;
}
.van-button::before {
  position: absolute;
  top: 50%;
  left: 50%;
  width: 100%;
  height: 100%;
  background-color: #000;
  border: inherit;
  border-color: #000;
  border-radius: inherit;
  -webkit-transform: translate(-50%, -50%);
  transform: translate(-50%, -50%);
  opacity: 0;
  content: " ";
}
</style>
