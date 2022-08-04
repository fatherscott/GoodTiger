# GoodTiger

c# Socket server.

It started for hu, but it's open to everyone.

If you ask questions or requests through Kakao Talk or issues, we'll review them and proceed.

Contact kakao https://open.kakao.com/o/g66zPHD

## Change Port

If you want to change the port, Change the value in appsetting.json:Port.

## Environmental variables

Add the appsetting.{type}.json and pass the entered {type} value to the ENVIRONMENT variable.

## Performance indicators

I measured Request Per Second.

It's not as fast as C++, but it's not bad.

![](https://github.com/fatherscott/GoodTiger/blob/main/image/RPS.PNG)

## GC Tuning

Checked that 2nd generation gc is not occurring

![](https://github.com/fatherscott/GoodTiger/blob/main/image/gc_tuning.PNG)
