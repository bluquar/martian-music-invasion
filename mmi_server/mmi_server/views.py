from django.http import HttpResponse
from django.http import HttpResponseBadRequest
import json
import os
import socket
from six import string_types

LOGFILE_DIRECTORY = {
    "chriss-macbook-pro-3.local": "/Users/chris/OneDrive/EdGames/MMI_Logs",
    "chriss-mbp-3.wv.cc.cmu.edu": "/Users/chris/OneDrive/EdGames/MMI_Logs",
    "Whatever My Windows Machine is called": "C:\Users\bluqu\Desktop\MMI Logs"
}[socket.gethostname().lower()]

SHARED_SECRET = "UgkLuhakJbBSAczdSisJHdn0PM02mK6W"

GREETING_ARG = 'HelloWorld'
LOGGING_ARG = "LogString"

SESSION_ID_TAG = "SessionID"
ACTION_TAG = "Action"

SESSION_START_ACTION = "Session_Start"
SESSION_END_ACTION = "Session_End"

SESSION_FILENAME_FORMAT = "session%08d.log"

def get_xml_tag(message, tag):
    assert(isinstance(message, string_types))
    assert(isinstance(tag, string_types))

    open_tag = "<%s>" % tag
    close_tag = "</%s>" % tag
    if (open_tag not in message) or (close_tag not in message):
        return None
    start_index = message.index(open_tag) + len(open_tag)
    end_index = message.index(close_tag)
    return message[start_index : end_index]

def get_json_value(message, key):
    d = json.loads(message)
    return d[key]

def get_session_id(message):
    session_id = get_json_value(message, SESSION_ID_TAG)
    assert(isinstance(session_id, string_types))
    return session_id

def is_session_start(message):
    action = get_json_value(message, ACTION_TAG)
    return action == SESSION_START_ACTION

def is_session_end(message):
    action = get_json_value(message, ACTION_TAG)
    return action == SESSION_END_ACTION

def greeting_response(post):
    print 'Greeting Received' % post
    return HttpResponse('')

def logging_response(message):
    d = json.loads(message)
    session_id = d[SESSION_ID_TAG]
    del d[SESSION_ID_TAG]

    logfilename = os.path.join(LOGFILE_DIRECTORY, "%s.json" % session_id)

    action = d.get(ACTION_TAG, None)
    is_start = not os.path.exists(logfilename)
    is_end = action == SESSION_END_ACTION

    out = open(logfilename, 'a')

    if is_start:
        out.write("[" + os.linesep + os.linesep)

    out.write(json.dumps(d))

    if is_end:
        os.write(os.linesep + "]")
    else:
        out.write("," + os.linesep)

    out.close()

    return HttpResponse("")

def log(request, argument=None):
    if argument == GREETING_ARG:
        print 'Greeting Received'
        return greeting_response(request.POST)

    elif argument == LOGGING_ARG:
        secret = request.POST.get('secret', 'NO SECRET')
        if secret != SHARED_SECRET:
            print '  Invalid Secret: %s' % secret
            return HttpResponseBadRequest('Authentication Failed')
        return logging_response(request.POST['message'])

    else:
        print 'Unrecognized Argument: %s' % argument
        return HttpResponseBadRequest("Unrecognized Argument")
