from django.http import HttpResponse
from django.http import HttpResponseBadRequest
import os

LOGFILE_DIRECTORY = "C:\Users\bluqu\Desktop\MMI Logs"
SHARED_SECRET = "UgkLuhakJbBSAczdSisJHdn0PM02mK6W"

GREETING_ARG = 'HelloWorld'

active_sessions = {}


def register_session(player_name):
    n = 0
    session_filename = os.path.join(LOGFILE_DIRECTORY, player_name + ".log")
    while os.path.exists(session_filename):
        session_filename = os.path.join(LOGFILE_DIRECTORY, player_name + (".%d.log" % n))
        n += 1
    active_sessions[player_name] = open(session_filename, 'w')


def session_log(player_name, message):
    if player_name not in active_sessions:
        print "Bad player name: %s" % player_name
        return

    active_sessions[player_name].write(message)


def greeting_response(post):
    print 'Greeting Post: %s' % post
    return HttpResponse('')


def log(request, argument=None):
    if argument == GREETING_ARG:
        print 'Greeting Received'
        return greeting_response(request.POST)

    print 'Log message -- Argument: %s' % argument

    if request.method == 'POST':
        secret = request.POST.get('secret', 'NO SECRET')
        if secret != SHARED_SECRET:
            print '  Invalid Secret: %s' % secret
            return HttpResponseBadRequest('Authentication Failed')
        print '  Log message -- POST Data: %s' % request.POST
    else:
        print '  Log message -- Non-POST Data: %s' % request.method

    return HttpResponse('')
