namespace Planilo.BT {

  /// <summary>AI graph result implementation for Behavior Trees.</summary>
  public class BTGraphResult : AIGraphResult {

    /// <summary>Internal list of valid result values.</summary>
    enum Results {
      /// <summary>The execution has failed.</summary>
      Failure,
      /// <summary>The execution has succeeded.</summary>
      Success,
      /// <summary>The execution is still running.</summary>
      Running
    }

    /// <summary>Internal value of the result.</summary>
    Results _result;

    /// <summary>Private contructor to set a particular value.</summary>
    BTGraphResult(Results result) {
      _result = result;
    }

    /// <summary>Get a new success result.</summary>
    public static BTGraphResult Success {
      get { return new BTGraphResult(Results.Success); }
    }

    /// <summary>Get a new failure result.</summary>
    public static BTGraphResult Failure {
      get { return new BTGraphResult(Results.Failure); }
    }

    /// <summary>Get a new running result.</summary>
    public static BTGraphResult Running {
      get { return new BTGraphResult(Results.Running); }
    }

    /// <summary>Invert the current result, running results stay unchanged.</summary>
    public BTGraphResult Invert() {
      if (_result == Results.Failure) {
        _result = Results.Success;
      } else if (_result == Results.Success) {
        _result = Results.Failure;
      }

      return this;
    }

    /// <summary>Is the current result a success.</summary>
    public bool IsSuccess {
      get { return _result == Results.Success; }
    }

    /// <summary>Is the current result a failure.</summary>
    public bool IsFailure {
      get { return _result == Results.Failure; }
    }

    /// <summary>Is the current result a running.</summary>
    public bool IsRunning {
      get { return _result == Results.Running; }
    }
  }
}