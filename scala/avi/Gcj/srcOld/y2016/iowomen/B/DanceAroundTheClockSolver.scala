package y2016.iowomen.B

import cmn._

object DanceAroundTheClockSolver extends GcjSolver() {
  override def solve(fetch: Fetch, out: Out) = {
    val List(cDancer, pDancer, cTurn) = fetch(PList(PBigInt))
    val i = pDancer - 1

    val shift = if (i % 2 == 0) BigInt(2) else cDancer - 2

    val iNext = (i + 1 + shift * cTurn) % cDancer
    val iPrev = (iNext + cDancer - 2) % cDancer

    out(iNext+1, iPrev+1)
  }
}
