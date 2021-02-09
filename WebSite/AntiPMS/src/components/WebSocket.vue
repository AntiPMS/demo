<template>
  <div>
    <!-- 消息通知 Start-->
    <van-notify v-model="showMsg"
                type="success">
      <van-icon name="bell"
                style="margin-right: 4px;" />
      <span>{{currentMsg}}</span>
    </van-notify>
    <!-- 消息通知 End-->
    <!-- 消息主体 Start-->
    <div style="height: 350px; background-color:#000;overflow-x: hidden;overflow-y:scroll;white-space:nowrap;">
      <van-list v-model="loading"
                :finished="finished"
                finished-text="没有更多了"
                direction="up"
                border=false
                @load="onLoadMsg">
        <van-cell v-for="msgtext in receiveMsgText"
                  :key="msgtext.Id">
          <div v-if="msgtext.isSelf==true"
               class="chat-bubble chat-bubble-right">
            <div>
              <div v-if="msgtext.msgType==1">
                {{msgtext.msg}}
              </div>
              <div v-else-if="msgtext.msgType==2">
                <img :src="msgtext.msg" />
              </div>
              <div v-else>
                {{msgtext.msg}}
              </div>
            </div>
          </div>
          <div v-else>
            <div>{{msgtext.senderName}}</div>
            <div class="chat-bubble chat-bubble-left">
              <div v-if="msgtext.msgType==1">
                {{msgtext.msg}}
              </div>
              <div v-else-if="msgtext.msgType==2">
                <img :src="msgtext.msg" />
              </div>
              <div v-else>
                {{msgtext.msgType}}
              </div>
            </div>
          </div>
        </van-cell>
      </van-list>
    </div>
    <!-- 消息主体 End-->
    <!-- 底部工具、输入框 Start -->
    <div ref="container"
         style="height: 150px;">
      <van-sticky :container="container">
        <van-cell-group>
          <van-cell>
            <van-row>
              <van-col span="12">
                <van-field v-model="sendMsg.senderName"
                           label="用户名：" />
              </van-col>
              <van-col span="6">
                <van-field v-model="sendMsg.targetId"
                           label="咨询号：" />
              </van-col>
            </van-row>
          </van-cell>
          <van-cell>
            <van-row>
              <van-col span="4">
                <van-icon name="smile-o"
                          size="35px" />
              </van-col>
              <van-col span="4">
                <van-uploader :after-read="upLoadFile">
                  <van-icon name="photo-o"
                            size="35px" />
                </van-uploader>
              </van-col>
              <van-col span="4">
                <van-icon name="orders-o"
                          size="35px" />
              </van-col>
            </van-row>
          </van-cell>
          <van-cell>
            <van-row>
              <van-col span="20">
                <van-field v-model="sendMsg.msg"
                           v-on:keyup.enter="doSend(true)"
                           type="textarea"
                           maxlength="150"
                           show-word-limit
                           placeholder="回车发送" />
              </van-col>
              <van-col span="4">
                <van-button v-on:click="doSend(true)"
                            type="info">发送</van-button>
              </van-col>
            </van-row>
          </van-cell>
        </van-cell-group>
      </van-sticky>
    </div>
    <!-- 底部工具、输入框 End -->
  </div>
</template>

<script>
export default {
  name: 'MyWebSocket',
  data () {
    return {
      currentMsg: '',
      showMsg: false,
      loading: false,
      finished: false,
      container: null,
      isEnterSend: true,
      socket: null,
      receiveMsgText: [],
      sendMsg: {
        Id: '',
        senderId: '1  ',
        senderName: '',
        targetId: '',
        msgType: 1,
        msg: ''
      }
    }
  },
  created: function () {
    // init sendMsg object
    this.sendMsg.senderId = 'qinko'// this.guid()
    this.sendMsg.senderName = 'qinko'// this.guid().substring(4)
    this.sendMsg.targetId = '3'
  },
  mouted: function () {
    this.container = this.$refs.container
  },
  beforeDestroy: function () {

  },
  destroy: function () {
    this.socket.onclose()
  },
  methods: {
    upLoadFile (file) {
      this.sendMsg.msgType = 2
      this.sendMsg.msg = file.content
      this.socket.send(JSON.stringify(this.sendMsg))
      this.sendMsg.msg = ''
    },
    showNotify (msg) {
      this.showMsg = true
      this.currentMsg = msg
      setTimeout(() => {
        this.showMsg = false
      }, 2000)
    },
    onLoadMsg () {
      this.doConnect(this)
      // 加载状态结束
      this.loading = false
      this.finished = true
    },
    onInput (checked) {
      this.isEnterSend = checked
    },
    guid () {
      function S4 () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1)
      }
      return (S4() + S4())
    },
    doConnect (app) {
      // cloudhospital.knjs.com.cn:1071
      let uri = 'ws://cloudhospital.knjs.com.cn:1071/ws?senderId=' + this.sendMsg.senderId + '&targetId=' + this.sendMsg.targetId
      this.socket = new WebSocket(uri)
      let showNotify = this.showNotify
      this.socket.onopen = function (e) {
        // app.receiveMsgText += 'opened!\n'
      }
      this.socket.onclose = function (e) {
        // app.receiveMsgText += 'closed!\n'
      }
      this.socket.onmessage = function (e) {
        var rmsg = JSON.parse(e.data)
        app.receiveMsgText.push(
          {
            isSelf: false,
            senderName: rmsg['SenderName'],
            msgType: rmsg['MsgType'],
            msg: rmsg['Msg']
          })
        showNotify(rmsg['Msg'])
      }
      this.socket.onerror = function (e) {
        app.receiveMsgText.push({
          isSelf: false,
          senderName: 'system',
          msg: '连接已断开'
        })
      }
    },
    doSend (isEnterSend) {
      if (isEnterSend) {
        this.sendMsg.msgType = 1
        this.socket.send(JSON.stringify(this.sendMsg))
        this.receiveMsgText.push(
          {
            Id: (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1),
            isSelf: true,
            senderName: this.sendMsg.senderName,
            msg: this.sendMsg.msg
          })
        this.sendMsg.msg = ''
      }
    }
  }
}

</script>

<style>
.chat-bubble {
  position: relative;
  margin: 12px;
  padding: 5px 8px;
  word-break: break-all;
  background: #fff;
  border: 1px solid #989898;
  border-radius: 5px;
  max-width: 180px;
}

.chat-bubble-left {
}

.chat-bubble-left:before {
  content: "";
  position: absolute;
  width: 0;
  height: 0;
  left: -20px;
  top: 5px;
  border: 10px solid;
  border-color: transparent #989898 transparent transparent;
}

.chat-bubble-left:after {
  content: "";
  position: absolute;
  width: 0;
  height: 0;
  left: -16px;
  top: 7px;
  border: 8px solid;
  border-color: transparent #ffffff transparent transparent;
}

.chat-bubble-right {
}

.chat-bubble-right:before {
  content: "";
  position: absolute;
  width: 0;
  height: 0;
  right: -20px;
  top: 5px;
  border: 10px solid;
  border-color: transparent transparent transparent #989898;
}

.chat-bubble-right:after {
  content: "";
  position: absolute;
  width: 0;
  height: 0;
  right: -16px;
  top: 7px;
  border: 8px solid;
  border-color: transparent transparent transparent #ffffff;
}

.chat-bubble-top {
}

.chat-bubble-top:before {
  content: "";
  position: absolute;
  width: 0;
  height: 0;
  left: 10px;
  top: 31px;
  border: 10px solid;
  border-color: #989898 transparent transparent transparent;
}

.chat-bubble-top:after {
  content: "";
  position: absolute;
  width: 0;
  height: 0;
  left: 12px;
  top: 30px;
  border: 8px solid;
  border-color: #ffffff transparent transparent transparent;
}

.chat-bubble-bottom {
}

.chat-bubble-bottom:before {
  content: "";
  position: absolute;
  width: 0;
  height: 0;
  right: 10px;
  top: -21px;
  border: 10px solid;
  border-color: transparent transparent #989898 transparent;
}

.chat-bubble-bottom:after {
  content: "";
  position: absolute;
  width: 0;
  height: 0;
  right: 12px;
  top: -16px;
  border: 8px solid;
  border-color: transparent transparent #ffffff transparent;
}
</style>
